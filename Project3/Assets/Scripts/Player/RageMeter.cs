using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// NOTE: Cooldown continues to fall once rageIncrease is inactive.
public class RageMeter : MonoBehaviour
{
    public Slider rageMeter;
    public float rageSpeed;
    public float rageCooldown;
    public bool rageIncrease;
    public float rageTime;

    private void Start()
    {
        rageCooldown = 1;
        rageIncrease = false;
    }

    void Update()
    {   if (rageTime > 0)
        {
            rageIncrease = true;
            rageMeter.value += rageSpeed * Time.deltaTime;
            rageTime -= Time.deltaTime;
        }
        if (rageTime <= 0) 
        {
            rageIncrease = false;
        }
        if (rageIncrease == true)
        {
            rageCooldown = 1;
        }
        if (rageIncrease == false)
        {
            rageCooldown -= Time.deltaTime;
        }
        if (rageCooldown <= 0)
        {
            rageIncrease = true;
        }



    }

    public void IsEnraged(float mod)
    {
        rageTime += mod;
    }
}
