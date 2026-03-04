using UnityEngine;

public class BeltCollectible : MonoBehaviour
{
	[SerializeField] int starNum;
	
	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
			switch(starNum)
			{
				case 1:
					StarTracker.star1get = true;
					break;
				case 2:
					StarTracker.star2get = true;
					break;
				case 3:
					StarTracker.star3get = true;
					break;
				default:
					Debug.Log("num " + starNum + "invalid");
					break;
			}
			
			Destroy(gameObject);
		}
    }
}
