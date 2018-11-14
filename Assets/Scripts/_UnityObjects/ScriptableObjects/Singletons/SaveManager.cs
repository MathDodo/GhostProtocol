using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveManager", menuName = "SaveManager", order = 0)]
public class SaveManager : RSingletonSO<SaveManager>
{
    private bool save;
    private bool alive;

    public override void OnInstantiated()
    {
        alive = true;
        Thread t = new Thread(() => SavingThread());
        t.IsBackground = true;
        t.Start();
    }

    public void StartSave()
    {
        if (!save)
            save = true;
    }

    private void SavingThread()
    {
        while (alive)
        {
            while (!save && alive) { }

            if (save)
            {
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        alive = false;
    }
}