using UnityEngine;

public class PowerGauge : MonoBehaviour
{
    public float currentMeter = 0f;
    public float maxMeter = 100f;

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


}
