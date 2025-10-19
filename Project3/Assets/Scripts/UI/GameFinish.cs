using UnityEngine;

public class GameFinish : MonoBehaviour
{
	[SerializeField] InGameMenuManager menuManager;
	
    private void OnTriggerEnter(Collider other)
    {
		menuManager.WinScreen();
	}
}
