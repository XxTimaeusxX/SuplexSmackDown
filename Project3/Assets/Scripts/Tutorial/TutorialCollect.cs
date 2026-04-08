using UnityEngine;
using UnityEngine.SceneManagement;
public class TutorialCollect : MonoBehaviour
{
   // public GameObject belts;
    private int beltsCollected = 0;
    private const int targetBelts = 2;
    public GameObject Window;
    public GameObject Window2;

    [Header("Scene Transition")]
    public int Scene1
        ; // Type the name of your next scene in the Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure your belt GameObjects are tagged with "Belt" in the Unity Editor
        if (other.CompareTag("Belt"))
        {
            beltsCollected++;
            Debug.Log($"Collected a belt! Total belts collected: {beltsCollected}");
            // Optional: Disable or destroy the belt object so it can't be collected again
            other.gameObject.SetActive(false); // or Destroy(other.gameObject);

            if (beltsCollected >= targetBelts)
            {
                Debug.Log("Successfully collected 2 belts!");
                Window.SetActive(false);
                Window2.SetActive(false);
            }
            if(beltsCollected == 3)
            {
                Debug.Log("Collected the first belt! Keep going!");
                SceneManager.LoadScene(Scene1);
            }
        }
    }
}
