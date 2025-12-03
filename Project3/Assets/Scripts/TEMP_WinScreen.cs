using UnityEngine;

public class TEMP_WinScreen : MonoBehaviour
{
    public InGameMenuManager gameMenuManager;

    private void OnTriggerEnter(Collider other)
    {
        gameMenuManager.WinScreen();
    }
}
