using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth;
	int currentHealth;
	public Texture2D[] healthSprites;
	public RawImage healthImg;
	[SerializeField] InGameMenuManager menuManager;
	
    void Start()
    {
		UpdateHealth(maxHealth);
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
