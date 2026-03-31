using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossShockwave : MonoBehaviour
{
    public float activeDuration = 2f;

    void Start()
    {
        transform.parent = null;
    }

    void Update()
    {
        activeDuration -= Time.deltaTime;
        if (activeDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
