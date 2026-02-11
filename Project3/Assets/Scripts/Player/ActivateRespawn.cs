using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnTrigger1;
    public GameObject respawnTrigger2;
    public DoorManager doorManager;
    public GameObject respawnZone1;
    public GameObject respawnZone2;
    public Transform playerRespawnPoint1;
    public Transform playerRespawnPoint2;
    public Transform playerRespawnPoint3;
    public bool respawn1;
    public bool respawn2;
    public bool respawn3;
    private PlayerMovement playerMovement;
    private CharacterController PlayerCC;
    private PlayerHealth playerHealth;
    private bool isRespawning = false;
    public bool falling;
    private float fallTime;
    public float maxFallTime;

    private void Start()
    {
        PlayerCC = GetComponent<CharacterController>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        falling = false;
        fallTime = maxFallTime;
        respawn1 = true;
        respawn2 = false;
        respawn3 = false;
    }
    private void Update()
    {
        if (playerMovement.isGrounded == false)
        {
            falling = true;
        }
        if (playerMovement.isGrounded)
        {
            falling = false;
            fallTime = maxFallTime;
        }
        if (falling)
        {
            fallTime -= Time.deltaTime;
        }
        if (fallTime <= 0)
        {
            if (respawn1)
            {
                RespawnPlayer(playerRespawnPoint1);
            }
            if (respawn2)
            {
                RespawnPlayer(playerRespawnPoint2);
            }
            if (respawn3)
            {
                RespawnPlayer(playerRespawnPoint3);
            }
                playerHealth.TakeDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider2"))
        {
            doorManager.close = true;
            Destroy(respawnTrigger1);
            respawnZone1.SetActive(true);
            respawn1 = false;
            respawn2 = true;
        }
        if (other.CompareTag("Collider3"))
        {
            Destroy(respawnTrigger2);
            respawnZone2.SetActive(true);
            respawn2 = false;
            respawn3 = true;
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
