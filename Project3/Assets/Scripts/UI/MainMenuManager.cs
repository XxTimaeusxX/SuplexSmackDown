using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] string _GameplayScene;
	
	public void StartButtonClicked(){
		Debug.Log("start!");
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
