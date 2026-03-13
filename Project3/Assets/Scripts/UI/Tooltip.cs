using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Tooltip : MonoBehaviour
{
	Text objText;
	[SerializeField] string newText;
	[SerializeField] bool destroyOnExit = false;
	[SerializeField] VideoClip videoClip;

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
		if (other.CompareTag("Player"))
        {
			tooltipVideoPlayer.clip = videoClip;
			objText.text = newText;
			tooltipPanel.SetActive(true);
			tooltipVideoPlayer.Play();
		}
	}
	
    private void OnTriggerExit(Collider other)
    {
		if (other.CompareTag("Player"))
        {
			tooltipPanel.SetActive(false);
			tooltipVideoPlayer.Stop();
			if(destroyOnExit)
				Destroy(this.gameObject);
		}
	}
}
