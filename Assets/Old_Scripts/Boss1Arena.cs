using UnityEngine;

public class Boss1Arena : MonoBehaviour
{
    public bool moveUp;
    public bool moveDown;
    public float moveSpeed;
    private float moveTime;
    public float maxMoveTime;

    private void Start()
    {
        moveTime = maxMoveTime;
    }

    void Update()
    {
        if (moveUp)
        {
            moveTime -= Time.deltaTime;
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            if (moveTime <= 0)
            {
                moveUp = false;
                moveTime = maxMoveTime;
            }
        }
        if (moveDown)
        {
            moveTime -= Time.deltaTime;
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            if (moveTime <= 0)
            {
                moveDown = false;
                gameObject.SetActive(false);
            }
        }
    }
}
