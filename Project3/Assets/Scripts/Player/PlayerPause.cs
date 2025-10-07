using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerPause : MonoBehaviour
{
    public PlayerInput playerInput;
	InputAction pauseAction;
	[SerializeField] GameObject _PauseMenuContainer;

	[SerializeField] GameObject _DefaultPauseButton;
	
    bool isPaused;
	
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseAction = playerInput.actions.FindAction("Pause");
		isPaused = false;
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            Debug.Log("Pause button pressed!");
			if (isPaused){
		        Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1.0f;
				_PauseMenuContainer.SetActive(false);
				isPaused = false;
			}
			else{
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
				_PauseMenuContainer.SetActive(true);
				isPaused = true;

                // Set default selected button for navigation
                EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
				
            }
        }
    }
}
