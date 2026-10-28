using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public bool open;
    public bool close;
    public bool ableToClose;

    private void Start()
    {
        open = false;
        close = false;
        ableToClose = true;
    }
}
