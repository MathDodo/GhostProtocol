using System.Threading;
using UnityEngine;

/// <summary>
/// Class used for reading and writing data
/// </summary>
[CreateAssetMenu(fileName = "SaveManger", menuName = "Singletons/SaveManager", order = 0)]
public class SaveManager : RSingletonSO<SaveManager>
{
    //Whether the saving thread should save
    private bool save;

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
        if (!save)
        {
            save = true;
        }
    }

    /// <summary>
    /// The thread continues to run and when it is time to save it will save
    /// </summary>
    private void SavingThread()
    {
        while (!destroyed)
        {
            while (!save) { }

            if (save)
            {
                Debug.Log("Saving");
                save = false;
            }
        }
    }
}