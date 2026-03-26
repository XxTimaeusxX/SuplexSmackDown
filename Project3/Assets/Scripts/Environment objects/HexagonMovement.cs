using UnityEngine;

public class HexagonMovement : MonoBehaviour
{
    public bool moveUp;
    public bool moveDown;
    public float speed;
    public float risingTime;
    public float loweringTime;

    private void Start()
    {
        risingTime = 1f;
        loweringTime = 1f;
    }

    private void Update()
    {
        if (moveUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            risingTime -= Time.deltaTime;
        }
        if (risingTime <= 0)
        {
            risingTime = 1f;
            moveUp = false;
            moveDown = true;
        }
        if (moveDown)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            loweringTime -= Time.deltaTime;
        }
        if (loweringTime <= 0)
        {
            loweringTime = 1f;
            moveDown = false;
        }
    }
}
