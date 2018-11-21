using SqlCipher4Unity3D;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

public class SQLiteDataWrapper
{
    private readonly SQLiteConnection _connection;

    public SQLiteDataWrapper(string DatabaseName)
    {
#if UNITY_EDITOR
        string dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
// check if file exists in Application.persistentDataPath
            string filepath = string.Format ("{0}/{1}", Application.persistentDataPath, DatabaseName);

            if (!File.Exists (filepath)) {
                Debug.Log ("Database not in Persistent path");
                // if it doesn't ->
                // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
                WWW loadDb = new WWW ("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName); // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) { } // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                // then save to Application.persistentDataPath
                File.WriteAllBytes (filepath, loadDb.bytes);
#elif UNITY_IOS
                string loadDb = Application.dataPath + "/Raw/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#elif UNITY_WP8
                string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);

#elif UNITY_WINRT
                string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#else
                string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);

#endif

			Debug.Log ("Database written");
		}

		var dbPath = filepath;
#endif
        this._connection = new SQLiteConnection(dbPath, "password");
        Debug.Log("Final PATH: " + dbPath);
    }

    public void CreateDB()
    {
        this._connection.DropTable<SavedData>();
        this._connection.CreateTable<SavedData>();
    }

    public void UpdateSave(SavedData data)
    {
        _connection.Update(data, data.GetType());
    }

    public SavedData GetSave(int id)
    {
        return this._connection.Table<SavedData>().Where(x => x.SaveID == id).FirstOrDefault();
    }

    public void InsertSave(SavedData savedData)
    {
        _connection.Insert(savedData);
    }
}