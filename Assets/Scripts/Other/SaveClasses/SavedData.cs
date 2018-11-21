using SQLite.Attribute;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class SavedData : IXmlSerializable
{
    [SerializeField]
    private int _saveID;

    [PrimaryKey]
    public int SaveID { get { return _saveID; } set { _saveID = value; } }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
    }

    public void WriteXml(XmlWriter writer)
    {
    }

    public SavedData CloneFromTemplate()
    {
        return (SavedData)MemberwiseClone();
    }
}