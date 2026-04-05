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
	bool isPlaying = false;

	[Header("From Main HUD")]
	[SerializeField] GameObject tooltipPanel;
	[SerializeField] GameObject tooltipText;
	[SerializeField] VideoPlayer tooltipVideoPlayer;
	BoxCollider col;
	
	void Start()
	{
		objText = tooltipText.GetComponent<Text>();
	}
	
    private void OnTriggerEnter(Collider other)
    {
		
		//if the player enters and it's not already active, start the coroutine to eventually disable it
		if (other.CompareTag("Player"))
        {
			if(!isPlaying){
				isPlaying = true;
				tooltipVideoPlayer.clip = videoClip;
				objText.text = newText;
				tooltipPanel.SetActive(true);
				tooltipVideoPlayer.Play();
				StartCoroutine("TimedDestroy");
			}
		}
	}
	
	IEnumerator TimedDestroy(){
		yield return new WaitForSeconds(destroyTime);
		tooltipPanel.SetActive(false);
		tooltipVideoPlayer.Stop();
		isPlaying = false;
		yield break; //break out of the coroutine
		//Destroy(this.gameObject);
	}
}
