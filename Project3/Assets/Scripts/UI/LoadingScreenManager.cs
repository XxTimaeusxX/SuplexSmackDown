using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
	
	[SerializeField] GameObject _LoadingScreenContainer;
	[SerializeField] Image _LoadingBar;
	private int sceneId = 0;
	[SerializeField] InGameMenuManager _inGameMenuManager;
	
	public void Start(){
		//StartLoadingScene(1); //TESTING ONLY - loads scene
	}
	
    public void StartLoadingScene(int newSceneId)
	{
		sceneId = newSceneId;
		_LoadingBar.fillAmount = 0;
		_LoadingScreenContainer.SetActive(true);
		if (_inGameMenuManager) _inGameMenuManager.HideAllUI();
		StartCoroutine("Load");
	}
	
	IEnumerator Load()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(2);
		
		while (!operation.isDone)
		{
			float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
			_LoadingBar.fillAmount = progressValue;
			yield return null;
		}
	}
}
