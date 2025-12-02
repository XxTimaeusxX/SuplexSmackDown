using System.Collections;
using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnTrigger1;
    public GameObject respawnTrigger2;
    public GameObject door;
    public GameObject respawnZone1;
    public GameObject respawnZone2;
    public Transform playerRespawnPoint1;
    public Transform playerRespawnPoint2;
    public bool respawn1 = false;
    public bool respawn2 = false;
    private PlayerMovement playerMovement;
    private CharacterController PlayerCC;
    private PlayerHealth playerHealth;
    private bool isRespawning = false;

    private void Start()
    {
        PlayerCC = GetComponent<CharacterController>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider2"))
        {
            door.SetActive(true);
            Destroy(respawnTrigger1);
            respawnZone1.SetActive(true);
            respawn1 = true;
        }
        if (other.CompareTag("Collider3"))
        {
            Destroy(respawnTrigger2);
            respawnZone2.SetActive(true);
            respawn2 = true;
        }
        if (other.CompareTag("Respawn1") && respawn1 == true)
        {
            RespawnPlayer(playerRespawnPoint1);
        }
        if (other.CompareTag("Respawn2") && respawn2 == true)
        {
            Debug.Log("Collide");
            RespawnPlayer(playerRespawnPoint2);
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
        isRespawning = false;
        PlayerCC.enabled = true;
    }
}
