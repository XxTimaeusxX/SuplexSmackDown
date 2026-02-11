using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth;
	public int currentHealth;
	public Texture2D[] healthSprites;
	public RawImage healthImg;
	public bool iFrames;
	private float iFrameCooldown;
	[SerializeField] InGameMenuManager menuManager;
    private bool isFirstHealthUpdate = true; // Flag to skip initial health sound
    void Start()
    {
        // Start the player with 1 HP (but never exceed maxHealth)
        int startHP = Mathf.Clamp(3, 0, maxHealth);
        UpdateHealth(startHP);
		iFrames = false;
		iFrameCooldown = 2f;
       // UpdateHealth(maxHealth);
    }

    private void Update()
    {
        if (iFrames == true)
        {
			iFrameCooldown -= Time.deltaTime;
        }
		if (iFrameCooldown <= 0)
		{
			iFrames = false;
			iFrameCooldown = 2f;
		}
    }

    public void TakeDamage()
    {
		UpdateHealth(--currentHealth);
		iFrames = true;
    }
	
	public void UpdateHealth(int newHP)
	{
		if(newHP >= 0){
			currentHealth = newHP;
			healthImg.texture = healthSprites[currentHealth];
		}
		if(newHP <= 0){
			GameOver();
			AudioManager.PlayGameOver();
		}
        // Skip health sound on first update (game start)
        if (isFirstHealthUpdate)
        {
            isFirstHealthUpdate = false;
            return;
        }
        switch (newHP)
		{
			case 3: AudioManager.PlayHealth3(); break;
			case 2: AudioManager.PlayHealth2(); break;
			case 1: AudioManager.PlayHealth1(); break;
			//case 0: GameOver(); AudioManager.PlayGameOver(); break;
        }
			Debug.Log("Current Health: " + currentHealth);
	}
	
	public void GameOver()
	{
		menuManager.GameOver();
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DamagePlayer") && iFrames == false)
		{
			TakeDamage();
			collision.gameObject.tag = "Macro";
		}
		if (collision.gameObject.CompareTag("Projectile") && iFrames == false)
		{
            TakeDamage();
        }
    }
}
