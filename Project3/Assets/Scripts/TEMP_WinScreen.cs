using UnityEngine;

public class TEMP_WinScreen : MonoBehaviour
{
    public InGameMenuManager gameMenuManager;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger win when the colliding GameObject's name is exactly "Micro"
        if (other != null && other.gameObject != null && other.gameObject.name == "Micro")
        {
            gameMenuManager.WinScreen();
        }
    }
}
