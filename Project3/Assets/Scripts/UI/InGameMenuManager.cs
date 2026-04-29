using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InGameMenuManager : MonoBehaviour
{
	[Header("-- Set these in-level --")]
	[SerializeField] CharacterController playerCC;
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] PlayerDash playerDash;
	[SerializeField] GameObject _StatusCanvas;
	
	[Header("-----------")]
	public PlayerInput playerInput;

	[SerializeField] int _MainMenuSceneInt = 0;
    [SerializeField] int _Stage1SceneInt = 1;
    [SerializeField] int _Stage2SceneInt = 2;
    [SerializeField] int _Stage3SceneInt = 3;
	[SerializeField] int _TutorialSceneInt = 4;

    [SerializeField] GameObject _PauseMenuContainer;
	[SerializeField] GameObject _HowToPlayPanel;
	[SerializeField] GameObject _SettingsPanel;
	[SerializeField] GameObject _WinMenuContainer;
	[SerializeField] GameObject _GameOverMenuContainer;
	[SerializeField] GameObject _CheatsMenu;
	[SerializeField] GameObject _CutscenePanel;
	
	[SerializeField] GameObject _PauseButtonContainer;
	[SerializeField] GameObject _DefaultPauseButton;
	[SerializeField] GameObject _DefaultHowToPlayButton;
	[SerializeField] GameObject _DefaultSettingsButton;
	[SerializeField] GameObject _DefaultWinButton;
	[SerializeField] GameObject _DefaultGameOverButton;
	[SerializeField] GameObject _DefaultCheatsButton;
	[SerializeField] GameObject _DefaultCutsceneButton;
	
	[SerializeField] GameObject _HealthUI;

	InputAction cheatsAction1;
	InputAction cheatsAction2;
	InputAction cheatsAction3;
	bool canInputCheats;

	[SerializeField] GameObject star1;
	[SerializeField] GameObject star2;
	[SerializeField] GameObject star3;
	
	float levelTimer = 0;
	float roundedTimer = 0;
	float minutes = 0;
	float seconds = 0;
	string formattedMinutes;
	string formattedSeconds;
	string formattedTimer = "00:00.0";
	[SerializeField] Text timerText;
	[SerializeField] Text winLevelTime;

	//[SerializeField] GameObject _PausePoster;
	Vector3 pauseMaxScale = new Vector3(1.5f, 1.5f, 1f);
	float pause_t = 0;

    bool isPaused = false;
	public bool canPause = true;
	
	[SerializeField] Animator pause_anim;
	
	//variables for gamma updates
	Image[] menuImg;
	Color[] menuImgColor;
	Image[] statusImg;
	Color[] statusImgColor;
	Text[] statusText;
	Color[] statusTextColor;
	TextMeshProUGUI[] statusText2;
	Color[] statusTextColor2;
	Text[] menuText;
	Color[] menuTextColor;
	RawImage[] rawImg;
	Color[] rawImgColor;
	float H,S,V;
	
	public void Start(){
        cheatsAction1 = playerInput.actions.FindAction("RainbowSuplex");
        cheatsAction2 = playerInput.actions.FindAction("Dash");
		//check to make sure the system can find the inputs
		if (cheatsAction1 != null && cheatsAction2 != null)
			canInputCheats = true;
		else{
			canInputCheats = false;
			Debug.Log("inputs not found!");
		}
		
		ColorInit();
		//TEST();	//DEBUG ONLY
	}
	
	public void TEST(){
		isPaused = true;
		Time.timeScale = 0.0f;
		Cursor.lockState = CursorLockMode.Confined;
	}
	
	//listen for cheat code
	public void Update()
	{
		//if all cheat inputs are pressed together, activate the cheats menu
		if (_SettingsPanel.active == true && canInputCheats && cheatsAction1.IsPressed() && cheatsAction2.IsPressed())
        {
			CheatsMenuActivate();
        }
		
		UpdateTimer();
	}
	
	
	public void UpdateTimer(){		
		if(seconds >= 5999.9){
			formattedTimer = "99:59.9"; //cap out timer if the player leaves it on or something
		}
		else{
			//update timer
			//increment levelTimer with deltaTime
			levelTimer += Time.deltaTime;
			roundedTimer = (Mathf.Round(levelTimer*10f)*0.1f);
			minutes = Mathf.Floor(roundedTimer/60);
			seconds = roundedTimer%60;
			//if at least 10 minutes, minutes to string, else add "0"
			formattedMinutes = minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString();
			//same for seconds
			formattedSeconds = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();
			formattedSeconds = seconds % 1 > 0 ? formattedSeconds : formattedSeconds + ".0";
			//format: 65.2 > 01:05.2
			formattedTimer = formattedMinutes.ToString() + ":" + formattedSeconds.ToString();
			//formattedTimer = roundedTimer.ToString();
			timerText.text = formattedTimer;
		}
	}
	
	//pause and show pause menu
	public void Pause()
	{
		if(canPause){
			//if already paused, act as ResumeButtonClicked
			if (isPaused){
				ResumeButtonClicked();
			}
			//pausing: play a sound, show cursor, set timeScale to 0, hide super suplex UI, and show pause menu
			else{
				playerCC.enabled = false; //prevent player input in the menu
				playerMovement.enabled = false;
				playerDash.enabled = false;
				
				AudioManager.PlaySuplexSlam();
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
				isPaused = true;
				_PauseMenuContainer.SetActive(true);
				_PauseButtonContainer.SetActive(true);
				_StatusCanvas.SetActive(false);
				if (pause_anim != null){
					pause_anim.SetTrigger("justPaused");
				}
				else 
					Debug.Log("no pause anim!");

				// Set default selected button for navigation
				EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
			}
		}
	}

	//lock and hide cursor, set timeScale to 1, show super suplex UI, and hide pause menu
	public void ResumeButtonClicked()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1.0f;
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_SettingsPanel) _SettingsPanel.SetActive(false);
		if(_HowToPlayPanel) _HowToPlayPanel.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
		if(_CheatsMenu) _CheatsMenu.SetActive(false);
		isPaused = false;
		_StatusCanvas.SetActive(true);
		pause_anim.SetBool("isPaused", false);
		
		playerCC.enabled = true; //allow the player to move again
		playerMovement.enabled = true;
		playerDash.enabled = true;
	}
	
	//when hovering over a button, set it to selected
	public void ButtonHover(GameObject curButton){
		EventSystem.current.SetSelectedGameObject(curButton);
		//Debug.Log("hovered " + curButton.name);
	}
	
	public void HowToPlayButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_HowToPlayPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultHowToPlayButton);
	}
	
	public void HowToPlayBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_HowToPlayPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	public void SettingsButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_SettingsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultSettingsButton);
	}
	
	public void SettingsBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_SettingsPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	//acts as ResumeButtonClicked and restart level
	public void RestartButtonClicked()
	{
		ResumeButtonClicked();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    PlaySceneMusic();
    }
	
	//unpause and return to main menu
	public void QuitButtonClicked()
	{
		isPaused = false;
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_MainMenuSceneInt);
	}

	//switch to cutscene button
	public void StartCutscene(){
		_WinMenuContainer.SetActive(false);
		_CutscenePanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultCutsceneButton);
	}
	
    //unpause and go to stage 1
	public void Stage1ButtonClicked()
	{
		_CutscenePanel.SetActive(false);
		isPaused = false;
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_Stage1SceneInt);
    }

    //unpause and go to stage 2
    public void Stage2ButtonClicked()
	{
		_CutscenePanel.SetActive(false);
		isPaused = false;
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_Stage2SceneInt);
    }
	
	//unpause and go to stage 3
	public void Stage3ButtonClicked()
	{
		_CutscenePanel.SetActive(false);
		isPaused = false;
		Time.timeScale = 1.0f;
        SceneManager.LoadScene(_Stage3SceneInt);
     //   PlaySceneMusic();
    }
	
	public void CheatsMenuActivate()
	{
		_SettingsPanel.SetActive(false);
		_CheatsMenu.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultCheatsButton);
	}
	
	//hide cheats menu and select settings
	public void CheatsBackButtonClicked()
	{
		_CheatsMenu.SetActive(false);
		_SettingsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultSettingsButton);
	}
	
	//show cursor, pause, and show game over menu
	public void GameOver()
	{
		canPause = false;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		_GameOverMenuContainer.SetActive(true);
		AudioManager.StopMusic();
		AudioManager.PlayDefeat();

        // Set default selected button for navigation
        EventSystem.current.SetSelectedGameObject(_DefaultGameOverButton);
	}
	
	public void WinScreen()
	{
		//show cursor, pause, and show win screen
		canPause = false;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		_WinMenuContainer.SetActive(true);
		
		//stop music and play victory sound
		AudioManager.StopMusic();
		AudioManager.PLayVictory();
        
		//show stars if obtained
		if(StarTracker.star1get)
			star1.SetActive(true);
		if(StarTracker.star2get)
			star2.SetActive(true);
		if(StarTracker.star3get)
			star3.SetActive(true);
		
		//show final time
		winLevelTime.text = "Final Time: " + formattedTimer;
		
		// Set default selected button for navigation
        EventSystem.current.SetSelectedGameObject(_DefaultWinButton);
		
	}
	
	//hides all UI - used in loading screen
	public void HideAllUI()
	{
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
		if(_HealthUI) _HealthUI.SetActive(false);
		if(_StatusCanvas) _StatusCanvas.SetActive(false);
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic();
    }
    //plays the appropriate music based on the current scene
    void PlaySceneMusic()
    {
        int currentSceneInt = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current Scene: " + currentSceneInt); // Add this line
        if (currentSceneInt == _MainMenuSceneInt)
        {
            AudioManager.PlayMainMenuBGM();
        }
		else if (currentSceneInt == _Stage1SceneInt)
		{
				AudioManager.PlayConstructionBGM();
        }
        else if (currentSceneInt == _Stage2SceneInt)
		{
			AudioManager.PlayFestivalBGM();
		}
		else if (currentSceneInt == _Stage3SceneInt)
		{
			AudioManager.PlayArenaBossBGM();
        }
        else if (currentSceneInt == _TutorialSceneInt)
        {
            AudioManager.PlayTutorialBGM();
        }
			
    }
	
	//play menu sounds, called in On Click () and Event Trigger (Select) in the inspector
	public void PlaySelectSound(){
		AudioManager.PlayMenuNavigateSelect();
	}
	
	public void PlayBackSound(){
		AudioManager.PlayMenuNavigateBack();
	}
	
	public void PlayNavigateSound(){
		AudioManager.PlayMenuNavigate();
	}
	
	//gamma updates
	void ColorInit(){
		menuImg = GetComponentsInChildren<Image>(true);
		menuText = GetComponentsInChildren<Text>(true);
		statusImg = _StatusCanvas.GetComponentsInChildren<Image>(true);
		statusText = _StatusCanvas.GetComponentsInChildren<Text>(true);
		statusText2 = _StatusCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
		
		menuImgColor = new Color[menuImg.Length];
		menuTextColor = new Color[menuText.Length];
		statusImgColor = new Color[statusImg.Length];
		statusTextColor = new Color[statusText.Length];
		statusTextColor2 = new Color[statusText2.Length];
		
		//paying for my early mistakes (but it's less trouble than going back and reverting everything to Image)
		rawImg = new RawImage[3];
		rawImgColor = new Color[3];
		RawImage[] temp1 = GetComponentsInChildren<RawImage>(true);
		//RawImage[] temp2 = _StatusCanvas.GetComponentsInChildren<RawImage>(true);
		rawImg[0] = temp1[0];
		rawImg[1] = temp1[1];
		rawImg[2] = temp1[2];
		
		print("StatusCanvas: ");
		foreach (TextMeshProUGUI newText in statusText2)
			print(newText.name);
		
		//loop through and initialize/sets colors, resetting index each time
		float newV;
		int index = 0;
		foreach (Image img in menuImg){
			menuImgColor[index] = img.color;
			img.color = GammaArrayUpdate(menuImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (RawImage img in rawImg){
			rawImgColor[index] = img.color;
			img.color = GammaArrayUpdate(rawImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (Image img in statusImg){
			statusImgColor[index] = img.color;
			img.color = GammaArrayUpdate(statusImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (Text newText in menuText){
			menuTextColor[index] = newText.color;
			newText.color = GammaArrayUpdate(menuTextColor, newText.color, index);
			index++;
		}
		index = 0;
		foreach (Text newText in statusText){
			statusTextColor[index] = newText.color;
			newText.color = GammaArrayUpdate(statusTextColor, newText.color, index);
			index++;
		}
		index = 0;
		foreach (TextMeshProUGUI newText in statusText2){
			statusTextColor2[index] = newText.color;
			newText.color = GammaArrayUpdate(statusTextColor2, newText.color, index);
			index++;
		}
	}
	
	Color GammaArrayUpdate(Color[] colorArray, Color newColor, int index){
		float newV;
		Color.RGBToHSV(colorArray[index], out H, out S, out V);
		newV = GammaCalc(V, PlayerPrefs.GetFloat("gamma"));
		return Color.HSVToRGB(H,S,newV);
	}
	
	//change value depending on slider
	float GammaCalc(float V, float newGamma){
		if(newGamma <= 0.05f)
			return V*(0.5f); //prevents everything from becoming too dark to see
		else if(newGamma <= 0.5f)
			return V*(newGamma+0.5f);
		else
			return 2*V*(newGamma);
	}
	
	//as ColorInit, but doesn't initialize anything
	public void GammaUIUpdate(float newGamma){
		int index = 0;
		float newV;
		foreach (Image img in menuImg){
			img.color = GammaArrayUpdate(menuImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (RawImage img in rawImg){
			img.color = GammaArrayUpdate(rawImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (Image img in statusImg){
			img.color = GammaArrayUpdate(statusImgColor, img.color, index);
			index++;
		}
		index = 0;
		foreach (Text newText in menuText){
			newText.color = GammaArrayUpdate(menuTextColor, newText.color, index);
			index++;
		}
		index = 0;
		foreach (Text newText in statusText){
			newText.color = GammaArrayUpdate(statusTextColor, newText.color, index);
			index++;
		}
		index = 0;
		foreach (TextMeshProUGUI newText in statusText2){
			newText.color = GammaArrayUpdate(statusTextColor2, newText.color, index);
			index++;
		}
	}
}
