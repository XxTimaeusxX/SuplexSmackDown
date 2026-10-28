using UnityEngine;
using UnityEngine.UI;


public class PowerGauge : MonoBehaviour
{
    private bool _infiniteMeterEnabled = false;
    public Slider powerSlider;
	[SerializeField] Image suplexBar;
	[SerializeField] Sprite suplexImg1;
	[SerializeField] Sprite suplexImgFull;
    public float rageSpeed;
    public float rageCooldown;
    public bool rageIncrease;

    private void Start()
    {
        rageCooldown = 1;
        rageIncrease = true;
    }

    void Update()
    {
        if (rageIncrease == true)
        {
            rageCooldown = 1;
        }
        if (powerSlider != null)
        {
            powerSlider.value += rageSpeed * Time.deltaTime;
        }
        if (rageIncrease == false)
        {
            rageCooldown -= Time.deltaTime;
        }
        if (rageCooldown <= 0)
        {
            rageIncrease = true;
        }

        if (powerSlider.value >= 1)
        {
            suplexBar.sprite = suplexImgFull;
        }
    }

    public void AddMeter(float amount)
    {
        if (_infiniteMeterEnabled) return; // If infinite meter is enabled, do not add meter

		if(powerSlider.value >= 1)
		{
			suplexBar.sprite = suplexImgFull;
		}
    }

    public bool SpendMeter()
    {
        if (_infiniteMeterEnabled) return false; // If infinite meter is enabled, dont spend meter

        if (powerSlider.value >= 1)
        {
            powerSlider.value = 0;
			suplexBar.sprite = suplexImg1;
            return true;
        }
        return false;
    }
    public void EnableInfiniteMeter()
    {
            _infiniteMeterEnabled = true;
        powerSlider.value = 1;
        suplexBar.sprite = suplexImgFull;
    }
    public void DisableInfiniteMeter()
    {
        _infiniteMeterEnabled = false;
    }

}
