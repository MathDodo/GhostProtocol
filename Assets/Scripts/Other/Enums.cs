/// <summary>
/// This enum will be used to have values for scenes in the future
/// </summary>
public enum SceneID
{
    SampleScene = 0
}

public enum Location
{
    None = 0,
    StreamingAssets = 1,
    PersistentData = 2
}

public enum SaveInstruction
{
    None = 0,
    Local = 1,
    NonLocal = 2
}

public enum SaveFileType
{
    None = 0,
    Json = 1,
    Xml = 2,
    SQliteDatabase = 3,
    Ini = 4,
    Txt = 5,
    Custom = 6
}

public enum ManageSaveFile
{
    None = 0,
    SingleFile = 1,
    MultipleFiles = 2
}

public enum FieldType
{
    None = 0,
    Int = 1,
    Float = 2,
    String = 3,
    Vector2 = 4,
    Vector3 = 5,
    Vector2Int = 6,
    Vector3Int = 7
}