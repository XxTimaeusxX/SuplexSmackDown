using UnityEngine;
using UnityEngine.UI;


public class PowerGauge : MonoBehaviour
{
    public float currentMeter = 0f;
    public float maxMeter = 100f;
    public Slider powerSlider;
	[SerializeField] Image suplexBar;
	[SerializeField] Sprite suplexImg1;
	[SerializeField] Sprite suplexImgFull;

    public void AddMeter(float amount)
    {
        currentMeter = Mathf.Clamp(currentMeter + amount, 0, maxMeter);
		if(currentMeter >= maxMeter)
		{
			suplexBar.sprite = suplexImgFull;
		}
    }

    public bool SpendMeter()
    {
        if (currentMeter >= maxMeter)
        {
            currentMeter -= maxMeter;
			suplexBar.sprite = suplexImg1;
            return true;
        }
        return false;
    }

    private void Update()
    {
        MeterGaugeUI();
    }

    private void MeterGaugeUI()
    {
        Mathf.Clamp(currentMeter, 0, maxMeter);
        powerSlider.value = currentMeter / 100;
    }
}
