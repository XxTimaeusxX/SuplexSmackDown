using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Respawn : MonoBehaviour
{
    private float groundLevel;
    public GameObject player;
    public List<GameObject> respawnTriggers;
    public List<Transform> respawnPoints;
    private PlayerMovement playerMovement;
    private CharacterController PlayerCC;
    private PlayerHealth playerHealth;
    public bool respawnPlayer;
    public int currentTriggerIndex;
    public int currentSpawnPointIndex;

    private void Start()
    {
        respawnPlayer = false;
        PlayerCC = GetComponent<CharacterController>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }
    private void Update()
    {
        if (transform.position.y < groundLevel)
        {
            respawnPlayer = true;
            RespawnPlayer(respawnPoints[currentSpawnPointIndex]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider"))
        {
            respawnTriggers[currentTriggerIndex].SetActive(false);
            currentTriggerIndex++;
            currentSpawnPointIndex++;
        }
    }

    public void RespawnPlayer(Transform newTransform)
    {
            StartCoroutine(TeleportPlayer(newTransform));
    }

    IEnumerator TeleportPlayer(Transform newTransform)
    {
        PlayerCC.enabled = false;
        yield return new WaitForSeconds(.1f);
        playerMovement.velocity.x = 0f;
        playerMovement.velocity.z = 0f;
        playerMovement.velocity.y = -2f;
        transform.position = newTransform.position;
        PlayerCC.enabled = true;
        if (respawnPlayer)
        {
            playerHealth.TakeDamage();
            respawnPlayer = false;
        }
    }
}
