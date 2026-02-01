using UnityEngine;
using UnityEngine.UI;


public class PowerGauge : MonoBehaviour
{
    public float currentMeter = 0f;
    public float maxMeter = 100f;
    private bool _infiniteMeterEnabled = false;
    public Slider powerSlider;
	[SerializeField] Image suplexBar;
	[SerializeField] Sprite suplexImg1;
	[SerializeField] Sprite suplexImgFull;

    public void AddMeter(float amount)
    {
        if (_infiniteMeterEnabled) return; // If infinite meter is enabled, do not add meter

        currentMeter = Mathf.Clamp(currentMeter + amount, 0, maxMeter);
		if(currentMeter >= maxMeter)
		{
			suplexBar.sprite = suplexImgFull;
		}
    }

    public bool SpendMeter()
    {
        if (_infiniteMeterEnabled) return false; // If infinite meter is enabled, dont spend meter

        if (currentMeter >= maxMeter)
        {
            currentMeter -= maxMeter;
			suplexBar.sprite = suplexImg1;
            return true;
        }
        return false;
    }
    public void EnableInfiniteMeter()
    {
            _infiniteMeterEnabled = true;
            currentMeter = maxMeter;
        suplexBar.sprite = suplexImgFull;
    }
    public void DisableInfiniteMeter()
    {
        _infiniteMeterEnabled = false;
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
