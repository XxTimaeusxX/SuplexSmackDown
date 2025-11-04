using UnityEngine;
using UnityEngine.UI;


public class PowerGauge : MonoBehaviour
{
    public float currentMeter = 0f;
    public float maxMeter = 100f;
    public Slider powerSlider;


    public void AddMeter(float amount)
    {
        currentMeter = Mathf.Clamp(currentMeter + amount, 0, maxMeter);
    }

    public bool SpendMeter()
    {
        if (currentMeter >= maxMeter)
        {
            currentMeter -= maxMeter;
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
