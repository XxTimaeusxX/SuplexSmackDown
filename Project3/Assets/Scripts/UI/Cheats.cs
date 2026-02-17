using UnityEngine;
using UnityEngine.UI;

public class Cheats : MonoBehaviour
{
	public InputField inputField;
	string submittedText;
	string formattedText;
	[SerializeField] Text feedbackText;
	
	[SerializeField] Color validColor;
	[SerializeField] Color invalidColor;
	
	[SerializeField] PlayerHealth playerHealth;
	
	void Start(){
		
	}
	
    //function to call from cheat menu's Input Field's On Submit
	public void SubmitCheat(){
		submittedText = inputField.text;
		formattedText = submittedText.Trim().ToUpper();
		Debug.Log("cheat submitted: " + formattedText);
		
		if(formattedText == "IRONHEAD"){
			bool isInvincible = playerHealth.ToggleInvincibility();
			if(isInvincible)
				feedbackText.text = "Invincible";
			else
				feedbackText.text = "No longer invincible";
		}
		
		inputField.text = ""; //clear text afterward
	}
	
}
