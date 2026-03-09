using UnityEngine;


public class Enable_boss : MonoBehaviour
{
    public GameObject RockyRhodes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RockyRhodes.SetActive(true);
            
        }



    }
}
