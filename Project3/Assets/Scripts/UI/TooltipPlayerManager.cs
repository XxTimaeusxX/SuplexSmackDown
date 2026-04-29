using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TooltipPlayerManager : MonoBehaviour
{
	Text objText;
	public bool isPlaying = false;
	[SerializeField] VideoPlayer tooltipVideoPlayer;
	[SerializeField] GameObject tooltipPanel;
	[SerializeField] GameObject tooltipText;
	
	void Start()
	{
		objText = tooltipText.GetComponent<Text>();
	}
	
    public void PlayTutorial(VideoClip videoClip, string newText)
    {
		if(!isPlaying){			
			isPlaying = true;
			tooltipVideoPlayer.clip = videoClip;
			objText.text = newText;
			tooltipPanel.SetActive(true);
			tooltipVideoPlayer.Play();
		}
    }
	
    public void StopTutorial()
    {	
		tooltipPanel.SetActive(false);
		tooltipVideoPlayer.Stop();
		isPlaying = false;
    }
}
