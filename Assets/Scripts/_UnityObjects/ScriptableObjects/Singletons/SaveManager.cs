using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

//[System.Serializable]
//public class XMLTest : IXmlSerializable
//{
//    [SerializeField]
//    private string test = "tut";

//    private string element = "uuu";

//    public XmlSchema GetSchema()
//    {
//        return null;
//    }

//    public void ReadXml(XmlReader reader)
//    {
//        while (reader.Read())
//        {
//            if (reader.NodeType == XmlNodeType.Element)
//            {
//                if (reader.HasAttributes)
//                {
//                    if (reader.GetAttribute("Save") == test)
//                    {
//                        Debug.Log(reader.ReadElementContentAsString());
//                    }
//                }
//            }
//        }
//    }

//    public void WriteXml(XmlWriter writer)
//    {
//        writer.WriteStartElement("Element");
//        writer.WriteAttributeString("Save", test);
//        writer.WriteString(element);
//        writer.WriteEndElement();
//    }

//    public override string ToString()
//    {
//        return test + " : " + element;
//    }
//}

/// <summary>
/// Class used for reading and writing data
/// </summary>
[CreateAssetMenu(fileName = "SaveManger", menuName = "Singletons/SaveManager", order = 0)]
public class SaveManager : RSingletonSO<SaveManager>
{
    //Whether the saving thread should save
    private bool _save;

    [SerializeField]
    private int _amountSaves = 1;

    [SerializeField]
    private string _saveFileName = "Placeholder";

    [SerializeField]
    private SavedData _template;

    [SerializeField]
    private Location _initialLocation;

    [SerializeField]
    private SaveInstruction _instruction = SaveInstruction.Local;

    [SerializeField]
    private SaveFileType _saveFileType = SaveFileType.Json;

    [SerializeField]
    private ManageSaveFile _management = ManageSaveFile.SingleFile;

    private SavedData _activeSave;
    private List<SavedData> _saves;

    public bool _LoadedData;
    public List<FieldCreator> _FieldCreators;

    public override void OnInstantiated()
    {
        Thread t = new Thread(SavingThread);
        t.IsBackground = true;
        t.Start();
    }

    /// <summary>
    /// Letting the thread save
    /// </summary>
    public void StartSave()
    {
        if (!_save)
        {
            _save = true;
        }
    }

    /// <summary>
    /// The thread continues to run and when it is time to save it will save
    /// </summary>
    private void SavingThread()
    {
        while (!_destroyed)
        {
            while (!_save && !_destroyed) { }

            if (_save)
            {
                Debug.Log("Saving");
                _save = false;
            }
        }
    }
}