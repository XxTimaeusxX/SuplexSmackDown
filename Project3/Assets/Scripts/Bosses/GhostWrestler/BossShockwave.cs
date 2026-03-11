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
        transform.position = new Vector3(transform.position.x, 27.62802f, transform.position.z);
        if (activeDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
