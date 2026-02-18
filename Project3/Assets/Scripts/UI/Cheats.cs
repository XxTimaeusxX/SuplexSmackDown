using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cheats : MonoBehaviour
{
	public InputField inputField;
	string submittedText;
	string formattedText;
	
	[SerializeField] Text feedbackText;
	[SerializeField] Color validColor;
	[SerializeField] Color invalidColor;
	[SerializeField] InGameMenuManager menuManager;
	
	[Header("Put Player Object Here")]
	[SerializeField] GameObject player;
	[SerializeField] CharacterController playerCC;
	[SerializeField] PlayerHealth playerHealth;
	
	[Header("Teleport Locations")]
	[SerializeField] GameObject MinibossLocation;
	[SerializeField] GameObject LevelBossLocation;
	
    //function to call from cheat menu's Input Field's On Submit
	public void SubmitCheat(){
		submittedText = inputField.text;
		formattedText = submittedText.Trim().ToUpper();
		
		feedbackText.color = validColor; //initializes to validColor each time; if it's invalid, set it to invalidColor later
		switch(formattedText)
		{
			// IRONHEAD - toggles invincibility
			case "IRONHEAD":
				bool isInvincible = playerHealth.ToggleInvincibility();
				if(isInvincible)
					feedbackText.text = "Invincible";
				else
					feedbackText.text = "No longer invincible";
				break;
			
			// SMALLFRY - teleport to miniboss
			case "SMALLFRY":
				StartCoroutine(Teleport(MinibossLocation.transform.position));
				break;
			
			// BIGCHEESE - teleport to level boss
			case "BIGCHEESE":
				StartCoroutine(Teleport(LevelBossLocation.transform.position));
				break;
			
			//if code is incorrect, set color to invalidColor and show an error
			default:
				feedbackText.color = invalidColor;
				feedbackText.text = "Invalid code";
				break;
		}
		
		Debug.Log(feedbackText.text);
		inputField.text = ""; //clear text afterward
	}
	
	//transports the player, then functions as ResumeButtonClicked
	IEnumerator Teleport(Vector3 newPosition){
		Time.timeScale = 1.0f;
		Debug.Log("Before Wait");
		yield return new WaitForSeconds(0.1f);
		Debug.Log("After Wait");
		player.transform.position = newPosition;
		menuManager.ResumeButtonClicked();
	}
	
	
}
