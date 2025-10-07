using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] string _GameplayScene;
	
	public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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
