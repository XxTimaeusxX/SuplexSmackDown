using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] string _GameplayScene;
    [SerializeField] GameObject _DefaultPlayButton;
   
    public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
    }
	
	public void StartButtonClicked(){
		Debug.Log("start!");
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		SceneManager.LoadScene(_GameplayScene);
	}
	
	public void ExitButtonClicked(){
		Debug.Log("exit!");
#if UNITY_EDITOR
	UnityEditor.EditorApplication.ExitPlaymode();
#else
	Application.Quit();
#endif
	}
}
