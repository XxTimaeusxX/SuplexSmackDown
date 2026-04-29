using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    [Header("Activate Respawns")]
    public GameObject actiavteRespawn1;
    public GameObject actiavteRespawn2;
    public GameObject actiavteRespawn3;

    [Header("RespawnTriggers")]
    public GameObject respawnTrigger1;
    public GameObject respawnTrigger2;
    public GameObject respawnTrigger3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == actiavteRespawn1)
        {
            respawnTrigger1.SetActive(true);
        }
        if(other.gameObject == actiavteRespawn2)
        {
            respawnTrigger2.SetActive(true);
        }
        if (other.gameObject == actiavteRespawn3)
        {
            respawnTrigger3.SetActive(true);
        }
    }
}
