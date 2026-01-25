using UnityEngine;

public class TEMP_WinScreen : MonoBehaviour
{
    public InGameMenuManager gameMenuManager;
    public GameObject micro;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger win when the colliding GameObject's name is exactly "Micro"
        if (other != null && other.gameObject != null && other.gameObject == micro)
        {
            gameMenuManager.WinScreen();
        }
    }
}
