using UnityEngine;

public sealed class StarTracker : MonoBehaviour
{
	public static StarTracker Instance { get; private set; }
	
	//holds whether the player has gotten the stars
	public static bool star1get;
	public static bool star2get;
	public static bool star3get;
	
	void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
	}
	
	//resets all stars; called on StartButtonClicked() in MainMenuManager
	public static void ResetStars(){
		star1get = false;
		star2get = false;
		star3get = false;
	}
	
}

