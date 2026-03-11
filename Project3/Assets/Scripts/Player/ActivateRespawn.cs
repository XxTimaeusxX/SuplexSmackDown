using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    private float groundLevel;
    public GameObject player;
    public GameObject respawnTrigger1;
    public GameObject respawnTrigger2;
    public GameObject respawnTrigger3;
    public GameObject respawnTrigger4;
    public GameObject respawnTrigger5;
    public List<GameObject> respawnTriggers;
    public List<Transform> respawnPoints;
    public Transform playerRespawnPoint1;
    public Transform playerRespawnPoint2;
    public Transform playerRespawnPoint3;
    public Transform playerRespawnPoint4;
    public Transform playerRespawnPoint5;
    public Transform playerRespawnPoint6;
    public bool respawn1;
    public bool respawn2;
    public bool respawn3;
    public bool respawn4;
    public bool respawn5;
    private MovementController movementController;
    public bool respawn6;
    private PlayerMovement playerMovement; // change
    private CharacterController PlayerCC;
    private PlayerHealth playerHealth;
    private SuplexController suplexController;
    private bool isRespawning = false;
    public bool falling;
    public float fallTime;
    public float maxFallTime;
    public bool turnOffFallDamage;

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
        //if (movementController.isGrounded == false)
        if (transform.position.y < groundLevel)
        {
        }
        // if (movementController.isGrounded)
        if (!turnOffFallDamage)
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
                if (respawn6)
                {
                    RespawnPlayer(playerRespawnPoint6);
                    fallTime = maxFallTime;
                }
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
            respawn4 = false;
            respawn5 = true;
        }
        if (other.CompareTag("Collider5"))
        {
            Destroy(respawnTrigger5);
            respawn5 = false;
            respawn6 = true;
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
