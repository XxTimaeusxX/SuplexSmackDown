using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerPause : MonoBehaviour
{
    public PlayerInput playerInput;
	InputAction pauseAction;
	[SerializeField] InGameMenuManager menuManager;
	
	[SerializeField] GameObject _PauseMenuContainer;

	[SerializeField] GameObject _DefaultPauseButton;
	
	
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseAction = playerInput.actions.FindAction("Pause");
    }

    void Update()
    {
        PauseInput();
    }
	
	void PauseInput()
    {
        if (pauseAction != null && pauseAction.WasPressedThisFrame() && menuManager.canPause)
        {
            Debug.Log("Pause button pressed!");
			menuManager.Pause();
        }
    }
}
