using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth;
	public int currentHealth;
	public Texture2D[] healthSprites;
	public RawImage healthImg;
	[SerializeField] InGameMenuManager menuManager;
	
    void Start()
    {
        // Start the player with 1 HP (but never exceed maxHealth)
        int startHP = Mathf.Clamp(1, 0, maxHealth);
        UpdateHealth(startHP);
       // UpdateHealth(maxHealth);
    }

    public void TakeDamage()
    {
		UpdateHealth(--currentHealth);
    }
	
	public void UpdateHealth(int newHP)
	{
		if(newHP >= 0){
			currentHealth = newHP;
			healthImg.texture = healthSprites[currentHealth];
		}
		if(newHP == 0){
			GameOver();
		}
			Debug.Log("Current Health: " + currentHealth);
	}
	
	public void GameOver()
	{
		menuManager.GameOver();
	}
}
