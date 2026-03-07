using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	[SerializeField] GameObject tooltipTextObj;
	Text objText;
	[SerializeField] string newText;
	[SerializeField] bool destroyOnExit = false;
	BoxCollider col;
	
	void Start()
	{
		objText = tooltipTextObj.GetComponent<Text>();
	}
	
    private void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player"))
        {
			objText.text = newText;
			tooltipTextObj.SetActive(true);
		}
	}
	
    private void OnTriggerExit(Collider other)
    {
		if (other.CompareTag("Player"))
        {
			tooltipTextObj.SetActive(false);
			if(destroyOnExit)
				Destroy(this.gameObject);
		}
	}
}
