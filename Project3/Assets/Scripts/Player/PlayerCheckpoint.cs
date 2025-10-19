using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
	public float killY = -15f;
	public Transform currentCheckpoint; //init with starting position
	private PlayerMovement playerMovement;
	private PlayerHealth playerHealth;
    
	void Start()
	{
		playerMovement = GetComponentInParent<PlayerMovement>();
		playerHealth = GetComponentInParent<PlayerHealth>();
	}
	
    void FixedUpdate()
    {
        CheckKillY();
    }
	
	private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered!");
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
		if (py <= killY){
			RespawnPlayer(currentCheckpoint);
			FallDamage();
		}
	}
	
	void FallDamage()
	{
		playerHealth.TakeDamage();
	}
	
	//stop player velocity and set player transform to last checkpoint
	public void RespawnPlayer(Transform newTransform)
	{
		playerMovement.velocity.x = 0f;
        playerMovement.velocity.z = 0f;
        playerMovement.velocity.y = -2f;
		transform.position = newTransform.position;
	}
}
