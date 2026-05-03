using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class Tooltip : MonoBehaviour
{
	Text objText;
	[SerializeField] string newText;
	[SerializeField] float destroyTime = 10.0f;
	[SerializeField] VideoClip videoClip;

	[Header("From Main HUD")]
	[SerializeField] TooltipPlayerManager tooltipPlayerManager;
	BoxCollider col;
	
	
    private void OnTriggerEnter(Collider other)
    {
		//if the player enters and it's not already active, start the coroutine to eventually disable it
		if (other.CompareTag("Player"))
        {
			if(!tooltipPlayerManager.isPlaying){
				tooltipPlayerManager.PlayTutorial(videoClip, newText);
				StartCoroutine("TimedDestroy");
			}
		}
	}
	
	IEnumerator TimedDestroy(){
		yield return new WaitForSeconds(destroyTime);
		tooltipPlayerManager.StopTutorial();
		yield break; //break out of the coroutine
	}
}
