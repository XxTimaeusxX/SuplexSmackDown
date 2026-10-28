using UnityEngine;

public class TriggerCameras : MonoBehaviour
{
    public Cinema_final cinemaFinalScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cinemaFinalScript == null) cinemaFinalScript = GetComponent<Cinema_final>();
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
