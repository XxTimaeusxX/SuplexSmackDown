using UnityEngine;
using System.Collections;

public class PlayerCheckpoint : MonoBehaviour
{
	public float killY = -15f;
	public Transform currentCheckpoint; //init with starting position
	private PlayerMovement playerMovement;
	private CharacterController PlayerCC;
	private PlayerHealth playerHealth;
	private bool isRespawning;
	
    void Start()
	{
		PlayerCC = GetComponent<CharacterController>();
		playerMovement = GetComponentInParent<PlayerMovement>();
		playerHealth = GetComponentInParent<PlayerHealth>();
		isRespawning = false;
	}
	
    void FixedUpdate()
    {
        CheckKillY();
    }
	
	private void OnTriggerEnter(Collider other)
    {
		//upon entering, set this checkpoint to currentCheckpoint
		if (other.CompareTag("Respawn"))
        {
			currentCheckpoint = other.transform;
        }
    }
	
	void CheckKillY()
	{
		var py = transform.position.y;
		
		//if player drops to killY, respawn the player at the last checkpoint
		if (!isRespawning && py <= killY){
			isRespawning = true;
			RespawnPlayer(currentCheckpoint);
		}
	}
	
	void FallDamage()
	{
		playerHealth.TakeDamage();
	}
	
	//stop player velocity and set player transform to last checkpoint
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
		FallDamage();
		isRespawning = false;
		PlayerCC.enabled = true;
	}
}
