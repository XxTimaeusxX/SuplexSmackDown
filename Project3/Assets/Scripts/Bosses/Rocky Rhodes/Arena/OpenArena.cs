using UnityEditor.EditorTools;
using UnityEngine;

public class OpenArena : MonoBehaviour
{
    public RockyRhodesManager manager;
    public bool leftSide;
    public bool rightSide;
    public float openTime;
    public float moveSpeed;
    public bool arena1, arena2;

    private void Update()
    {
        Arena1();
        Arena2();
    }

    private void LeftSideOpen()
    {
        if (leftSide)
        {
            if (manager.open)
            {
                if (arena1)
                {
                    openTime -= Time.deltaTime;
                    transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }
                if (arena2)
                {
                    openTime -= Time.deltaTime;
                    transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
                }
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
                if (arena1)
                {
                    openTime -= Time.deltaTime;
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
                if (arena2)
                {
                    openTime -= Time.deltaTime;
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                }
            }
        }
        if (openTime <= 0)
        {
            manager.open = false;
        }
    }

    private void Arena1()
    {
        if (arena1 && manager.arena1)
        {
            LeftSideOpen();
            RightSideOpen();
        }
    }

    private void Arena2()
    {
        if (arena2 && manager.arena2)
        {
            LeftSideOpen();
            RightSideOpen();
        }
    }
}
