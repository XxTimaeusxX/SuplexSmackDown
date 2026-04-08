using UnityEngine;

public class EnableGrandeYpequeno : MonoBehaviour
{
    public Collider bossTrigger;
    public GameObject GrandeYPequeno;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     GrandeYPequeno.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GrandeYPequeno.SetActive(true);
            Destroy(bossTrigger);
        }
    }
}

