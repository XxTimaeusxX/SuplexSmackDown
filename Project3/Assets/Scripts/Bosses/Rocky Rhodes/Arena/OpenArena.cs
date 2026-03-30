using UnityEditor.EditorTools;
using UnityEngine;

public class OpenArena : MonoBehaviour
{
    public RockyRhodesManager manager;
    public bool leftSide;
    public bool rightSide;
    public float openTime;
    public float moveSpeed;

    private void Update()
    {
        LeftSideOpen();
        RightSideOpen();
    }

    private void LeftSideOpen()
    {
        if (leftSide)
        {
            if (manager.open)
            {
                openTime -= Time.deltaTime;
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
        }
        if (openTime <= 0)
        {
            manager.open = false;
        }
    }

    private void RightSideOpen()
    {
        if (rightSide)
        {
            if (manager.open)
            {
                openTime -= Time.deltaTime;
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
        }
        if (openTime <= 0)
        {
            manager.open = false;
        }
    }
}
