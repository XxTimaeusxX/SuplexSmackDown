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
        int startHP = Mathf.Clamp(3, 0, maxHealth);
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
			AudioManager.PlayGameOver();
		}
	   	switch(newHP)
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
}
