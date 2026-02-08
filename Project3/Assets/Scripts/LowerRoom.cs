
using UnityEngine;

public class LowerRoom : MonoBehaviour
{
    float speed = 2.0f;
    public Vector3 TargetDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetDistance = transform.position + Vector3.down * 5.0f - Vector3.right * 7.0f;
    }

    // Update is called once per frame

    public void MoveDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetDistance, speed * Time.deltaTime);
    }
}
