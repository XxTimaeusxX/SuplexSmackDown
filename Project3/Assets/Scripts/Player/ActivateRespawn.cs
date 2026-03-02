using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnTrigger1;
    public GameObject respawnTrigger2;
    public GameObject respawnTrigger3;
    public GameObject respawnTrigger4;
    public DoorManager doorManager;
    public Transform playerRespawnPoint1;
    public Transform playerRespawnPoint2;
    public Transform playerRespawnPoint3;
    public Transform playerRespawnPoint4;
    public Transform playerRespawnPoint5;
    public bool respawn1;
    public bool respawn2;
    public bool respawn3;
    public bool respawn4;
    public bool respawn5;
    private MovementController movementController;
    private CharacterController PlayerCC;
    private PlayerHealth playerHealth;
    private SuplexController suplexController;
    private bool isRespawning = false;
    public bool falling;
    public float fallTime;
    public float maxFallTime;

    private void Start()
    {
        PlayerCC = GetComponent<CharacterController>();
        movementController = GetComponent<MovementController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        suplexController = GetComponent<SuplexController>();
        falling = false;
        fallTime = maxFallTime;
        respawn1 = true;
        respawn2 = false;
        respawn3 = false;
    }
    private void Update()
    {
        if (movementController.isGrounded == false)
        {
            falling = true;
        }
        if (movementController.isGrounded)
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
                fallTime = maxFallTime;
            }
            if (respawn2)
            {
                RespawnPlayer(playerRespawnPoint2);
                fallTime = maxFallTime;
            }
            if (respawn3)
            {
                RespawnPlayer(playerRespawnPoint3);
                fallTime = maxFallTime;
            }
            if (respawn4)
            {
                RespawnPlayer(playerRespawnPoint4);
                fallTime = maxFallTime;
            }
            if (respawn5)
            {
                RespawnPlayer(playerRespawnPoint5);
                fallTime = maxFallTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider1"))
        {
            Destroy(respawnTrigger1);
            respawn1 = false;
            respawn2 = true;
        }
        if (other.CompareTag("Collider2"))
        {
            Destroy(respawnTrigger2);
            respawn2 = false;
            respawn3 = true;
        }
        if (other.CompareTag("Collider3"))
        {
            Destroy(respawnTrigger3);
            respawn3 = false;
            respawn4 = true;
        }
        if (other.CompareTag("Collider4"))
        {
            Destroy(respawnTrigger4);
            respawn3 = false;
            respawn4 = true;
        }
    }

    public void RespawnPlayer(Transform newTransform)
    {
        StartCoroutine(TeleportPlayer(newTransform));
        playerHealth.TakeDamage();
    }

    IEnumerator TeleportPlayer(Transform newTransform)
    {
        PlayerCC.enabled = false;
        yield return new WaitForSeconds(.1f);
        movementController.velocity.x = 0f;
        movementController.velocity.z = 0f;
        movementController.velocity.y = -2f;
        transform.position = newTransform.position;
        isRespawning = false;
        PlayerCC.enabled = true;
    }
}
