using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] int _GameplaySceneInt;
    [SerializeField] GameObject _DefaultPlayButton;
    [SerializeField] LoadingScreenManager _loadingScreenManager;
   
    public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
    }
	
	public void StartButtonClicked(){
		Debug.Log("start!");
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		_loadingScreenManager.StartLoadingScene(_GameplaySceneInt);
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
