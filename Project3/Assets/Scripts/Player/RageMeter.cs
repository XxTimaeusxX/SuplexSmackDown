using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        }
        if (rageTime <= 0) 
        {
            rageIncrease = false;
        }
        if (rageIncrease == true)
        {
            rageCooldown = 1;
        }
        if (rageMeter != null)
        {
            rageMeter.value += rageSpeed * Time.deltaTime;
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
