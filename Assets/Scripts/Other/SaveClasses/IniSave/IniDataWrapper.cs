using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IniDataWrapper
{
    private const string K_DEFAULT_DELIITER = ";";
    private const string K_COMMANDS_DELIMITER = "#";

    // Commands
    private const string K_COMMAND_VERSION = "version";

    private const string K_COMMAND_COMMENTS = "comments";

    /// <summary>
    /// The initialization file path.
    /// </summary>
    private string theFile = null;

    /// <summary>
    /// The qualifiers to apply when parsing the ini file
    /// </summary>
    private List<string> theQualifiers = new List<string>();

    private HashSet<string> mSections = new HashSet<string>();

    // "[section]key"   -> "value1"
    // "[section]key~2" -> "value2"
    // "[section]key~3" -> "value3"
    private Dictionary<string, string> dictionary = new Dictionary<string, string>();

    public List<string> Qualifiers
    {
        get { return theQualifiers; }

        set
        {
            theQualifiers = value;
            // if an ini file was already specified, forcibly reparse it
            if (null != theFile)
            {
                TheFile = theFile;
            }
        }
    }

    public int Version { get; private set; }

    /// <summary>
    /// The comment delimiter string (default value is ";").
    /// </summary>
    public string CommentDelimiter { get; set; }

    public string CommandsDelimiter { get; set; }

    public string TheFile { get { return theFile; } set { theFile = value; } }

    /// <summary>
    /// Gets a string value by section and key.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    /// <seealso cref="GetValue"/>
    public string this[string section, string key]
    {
        get
        {
            return GetValue(section, key);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IniDataWrapper"/> class.
    /// </summary>
    public IniDataWrapper()
    {
        CommentDelimiter = K_DEFAULT_DELIITER;
        CommandsDelimiter = K_COMMANDS_DELIMITER;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IniDataWrapper"/> class.
    /// </summary>
    /// <param name="file">The initialization file path.</param>
    /// <param name="commentDelimiter">The comment delimiter string (default value is "#").
    /// </param>
    public IniDataWrapper(string file, string commentDelimiter = K_DEFAULT_DELIITER, string commandsDelimiter = K_COMMANDS_DELIMITER)
    {
        CommentDelimiter = commentDelimiter;
        CommandsDelimiter = commandsDelimiter;
        TheFile = file;
    }

    public Dictionary<string, string> GetTerms()
    {
        return dictionary;
    }

    public void SetValue(string section, string term, string value)
    {
        AddValue(section, term, value);
    }

    public void AddValue(string section, string term, string value)
    {
        dictionary[GetFinalTerm(section, term)] = value;
    }

    public void DeleteValue(string section, string term)
    {
        string finalTerm = GetFinalTerm(section, term);

        if (dictionary.ContainsKey(finalTerm))
        {
            dictionary.Remove(term);
        }
    }

    public void Export()
    {
        Export(TheFile);
    }

    public void Export(string file)
    {
        UnityEngine.Debug.Assert(file != null);

        if (file == null)
        {
            return;
        }

        StreamWriter writeFile;
        writeFile = File.CreateText(file);

        // Retrieve all dictionary sections and entries per section.
        Dictionary<string, List<KeyValuePair<string, string>>> sections = new Dictionary<string, List<KeyValuePair<string, string>>>();

        foreach (var entry in dictionary)
        {
            var terms = entry.Key.Split('|');

            if (terms.Length == 2)
            {
                string section = terms[0];
                string term = terms[1];

                List<KeyValuePair<string, string>> entriesForSection;

                if (!sections.TryGetValue(section, out entriesForSection))
                {
                    entriesForSection = new List<KeyValuePair<string, string>>();
                    sections.Add(section, entriesForSection);
                }

                entriesForSection.Add(new KeyValuePair<string, string>(term, entry.Value));
            }
        }

        // Write it all down
        foreach (var section in sections)
        {
            //Put on encryption?
            writeFile.WriteLine("[" + section.Key + "]");

            for (int i = 0; i < section.Value.Count; i++)
            {
                string term = section.Value[i].Key;
                string value = section.Value[i].Value;

                //Put on encryption?
                writeFile.WriteLine(term + "=" + value);
            }

            writeFile.WriteLine(string.Empty);
        }

        writeFile.Close();
    }

    public void LoadFromText(string text)
    {
        theFile = "";
        dictionary.Clear();
        mSections.Clear();
        ImportString(text);
    }

    public void UpdateWith(IniDataWrapper other)
    {
        foreach (var entry in other.dictionary)
        {
            var key = entry.Key;
            dictionary[key] = entry.Value;
        }
    }

    public void Clear()
    {
        dictionary.Clear();
        mSections.Clear();
    }

    private string GetFinalTerm(string section, string term)
    {
        return section.ToLower() + "|" + term.ToLower();
    }

    private void ImportString(string text)
    {
        bool commentsOnLines = false;

        using (StringReader sr = new StringReader(text))
        {
            string line, section = "";

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.Length == 0)
                {
                    continue;  // empty line
                }

                if (!string.IsNullOrEmpty(CommentDelimiter) && line.StartsWith(CommentDelimiter))
                {
                    continue;  // comment
                }

                if (!string.IsNullOrEmpty(CommandsDelimiter) && line.StartsWith(K_COMMANDS_DELIMITER))
                {
                    var commandClump = line.Substring(1).Trim().ToLower();
                    var commands = commandClump.Split(' ');

                    if (commands.Length > 0)
                    {
                        var command = commands[0];

                        if (command == K_COMMAND_COMMENTS)
                        {
                            commentsOnLines = true;
                        }

                        if (command == K_COMMAND_VERSION)
                        {
                            if (commands.Length == 2)
                            {
                                int v;
                                commands[1].TryParseInt(out v);
                                Version = v;
                            }
                        }
                    }

                    continue; // command!
                }

                if (line.StartsWith("[") && line.Contains("]"))  // [section]
                {
                    int index = line.IndexOf(']');
                    //Decrypt section?
                    section = line.Substring(1, index - 1).Trim().ToLower();
                    // reset special commands
                    commentsOnLines = false;

#if UNITY_EDITOR
                    // we don't allowy repeating sections, so put some warnings
                    if (mSections.Contains(section))
                    {
                        UnityEngine.Debug.LogWarning("Section was listed 2x in file: " + theFile + " section: " + section);
                    }
#endif
                    continue;
                }

                if (line.Contains("="))  // key=value
                {
                    // Special commands
                    if (commentsOnLines)
                    {
                        // If we allow comments on the same line, strip them.
                        if (line.Contains(CommentDelimiter))
                        {
                            int v = line.IndexOf(';');
                            line = line.Substring(0, v);
                        }
                    }

                    int index = line.IndexOf('=');

                    //Decrypt key?
                    string key = line.Substring(0, index).Trim();
                    var qualifiers = ParseQualifiers(ref key);

                    // if qualifiers are specified, we only use this if they ALL match
                    if (null != qualifiers)
                    {
                        if (!qualifiers.All((q) =>
                        {
                            return theQualifiers.Contains(q);
                        }))
                        {
                            continue;
                        }
                    }

                    //Decrypt val?
                    string val = line.Substring(index + 1).Trim();
                    string key2 = GetFinalTerm(section, key);

                    if (val.StartsWith("\"") && val.EndsWith("\""))  // strip quotes
                    {
                        val = val.Substring(1, val.Length - 2);
                    }

                    // if it was already there, replace... last entry wins (allows inheritance)
                    if (dictionary.ContainsKey(key2))
                    {
                        dictionary[key2] = val;
#if UNITY_EDITOR
                        UnityEngine.Debug.LogWarning("We had a duplicate entry in ini file: " + theFile + " section: " + section + " key: " + key);
#endif
                    }
                    else
                    {
                        dictionary.Add(key2, val);
                        if (!mSections.Contains(section))
                            mSections.Add(section);
                    }
                }
            }
        }
    }

    public void ImportFile(string file)
    {
        theFile = file;
        dictionary.Clear();
        mSections.Clear();

        if (File.Exists(theFile))
        {
            using (StreamReader sr = new StreamReader(file))
            {
                ImportString(sr.ReadToEnd());
            }
        }
    }

    /// <summary>
    /// Splits the actual key from it's qualifiers and returns both parts.
    /// </summary>
    /// <returns>The key.</returns>
    /// <param name="origKey">Original key.</param>
    /// <param name="qualifiers">Any qualifiers (or null if there are none)</param>
    private string[] ParseQualifiers(ref string origKey)
    {
        // the format is a '.'-separated list of qualifiers
        if (origKey.Contains("."))
        {
            var terms = origKey.Split('.');
            origKey = terms[0];
            return new List<string>(terms).GetRange(1, terms.Length - 1).ToArray();
        }

        return null;
    }

    private bool TryGetValue(string section, string key, out string value)
    {
        string key2 = GetFinalTerm(section, key);
        //String.Format("{0}|{1}", section, key);

        return dictionary.TryGetValue(key2.ToLower(), out value);
    }

    /// <summary>
    /// Gets a string value by section and key.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value.</returns>
    public string GetValue(string section, string key, string defaultValue = "")
    {
        string value;

        if (!TryGetValue(section, key, out value))
        {
            return defaultValue;
        }

        return value;
    }

    public string[] GetValuesList(string section, string key)
    {
        var value = GetValue(section, key);
        return ParseStringAsValueList(value);
    }

    public int[] GetIntList(string section, string key)
    {
        var values = GetValuesList(section, key);
        int[] intValues = new int[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            float work;
            values[i].TryParseFloat(out work);
            intValues[i] = (int)work;
        }

        return intValues;
    }

    public UnityEngine.Color GetRGBColor(string section, string key, UnityEngine.Color defaultColor)
    {
        string value;

        if (TryGetValue(section, key, out value))
        {
            string[] rgb = value.Split(',');

            if (rgb.Length != 3)
            {
                return defaultColor;
            }

            int[] color = new int[rgb.Length];

            for (int i = 0; i < rgb.Length; i++)
            {
                if (!rgb[i].TryParseInt(out color[i]))
                    return defaultColor;
            }

            return new UnityEngine.Color32((byte)color[0], (byte)color[1], (byte)color[2], 255);
        }
        else
        {
            return defaultColor;
        }
    }

    public bool HasSection(string section)
    {
        section = section.ToLower();
        return mSections.Contains(section);
    }

    public string[] GetSections()
    {
        return mSections.ToArray();
    }

    /// <summary>
    /// Return all of the available
    /// </summary>
    /// <returns>The keys.</returns>
    /// <param name="section">Section.</param>
    public string[] GetKeys(string section = "")
    {
        List<string> matches = new List<string>();
        var sectionPrefix = String.Format("{0}|", section.ToLower());

        foreach (var item in dictionary)
        {
            if (string.IsNullOrEmpty(section) || item.Key.StartsWith(sectionPrefix))
            {
                matches.Add(item.Key.Substring(sectionPrefix.Length));
            }
        }

        return matches.ToArray();
    }

    public KeyValuePair<string, string>[] GetKeysAndValues(string section = "")
    {
        List<KeyValuePair<string, string>> matches = new List<KeyValuePair<string, string>>();
        var sectionPrefix = String.Format("{0}|", section.ToLower());

        foreach (var item in dictionary)
        {
            if (string.IsNullOrEmpty(section) || item.Key.StartsWith(sectionPrefix))
            {
                matches.Add(new KeyValuePair<string, string>(item.Key.Substring(sectionPrefix.Length), item.Value));
            }
        }

        return matches.ToArray();
    }

    /// <summary>
    /// Gets an integer value by section and key.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="minValue">Optional minimum value to be enforced.</param>
    /// <param name="maxValue">Optional maximum value to be enforced.</param>
    /// <returns>The value.</returns>
    public int GetInteger(string section, string key, int defaultValue = 0, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        string stringValue;

        if (!TryGetValue(section, key, out stringValue))
        {
            return defaultValue;
        }

        int value;

        if (!stringValue.TryParseInt(out value))
        {
            float dvalue;

            if (!stringValue.TryParseFloat(out dvalue))
            {
                return defaultValue;
            }

            value = (int)dvalue;
        }

        if (value < minValue)
        {
            value = minValue;
        }
        else if (value > maxValue)
        {
            value = maxValue;
        }

        return value;
    }

    /// <summary>
    /// Gets a double floating-point value by section and key.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="minValue">Optional minimum value to be enforced.</param>
    /// <param name="maxValue">Optional maximum value to be enforced.</param>
    /// <returns>The value.</returns>
    public float GetFloat(string section, string key, float defaultValue = 0, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        string stringValue;

        if (!TryGetValue(section, key, out stringValue))
        {
            return defaultValue;
        }

        float value;

        if (!stringValue.TryParseFloat(out value))
        {
            return defaultValue;
        }

        if (value < minValue)
        {
            value = minValue;
        }

        if (value > maxValue)
        {
            value = maxValue;
        }

        return value;
    }

    /// <summary>
    /// Gets a boolean value by section and key.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value.</returns>
    public bool GetBoolean(string section, string key, bool defaultValue = false)
    {
        string stringValue;

        if (!TryGetValue(section, key, out stringValue))
        {
            return defaultValue;
        }

        return (stringValue != "0" && !stringValue.StartsWith("f", true, null));
    }

    public void Reset()
    {
        TheFile = null;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var value in dictionary)
        {
            sb.AppendFormat("{0}={1}\n", value.Key, value.Value);
        }

        return sb.ToString();
    }

    public static string[] ParseStringAsValueList(string value, char delimiter = ',')
    {
        if (value == string.Empty)
        {
            return new string[0];
        }

        var val = value.Split(delimiter);

        for (int i = 0; i < val.Length; i++)
        {
            val[i] = val[i].Trim();
        }

        return val;
    }
}