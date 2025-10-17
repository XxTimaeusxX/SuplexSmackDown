using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth;
	int currentHealth;
	public Texture2D[] healthSprites;
	public RawImage healthImg;
	
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
			Debug.Log("Current Health: " + currentHealth);
	}
}
