using System.Collections;
using UnityEngine;

public class RockyArena2Attacks : MonoBehaviour
{
    RockyActionManager manager;

    private void Awake()
    {
        manager = GetComponent<RockyActionManager>();
    }

    #region Cannonball
    public void StartCannonball()
    {
        if (manager.cannonball && manager.canPerformAction && !manager.grabbed)
        {
            gameObject.tag = "Rocky Rhodes";
            Jump();
            if (manager.canChooseRandom)
            {
                ChooseRandomTile();
            }
        }
    }

    private void Jump()
    {
        manager.agent.enabled = false;
        manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.jumpForce);
        StartCoroutine(JumpTime(manager.jumpTime));
    }

    private IEnumerator JumpTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SlamDown();
    }

    private void ChooseRandomTile()
    {
        manager.canChooseRandom = false;
        if (manager.tiles == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.tiles.Length);
        manager.chosenPoint = manager.tiles[randomPoint];
    }

    private void SlamDown()
    {
        manager.cannonball = false;
        float step = manager.slamForce * Time.deltaTime;
        Vector3 targetWithHeight = new Vector3(manager.chosenPoint.position.x, manager.chosenPoint.position.y + manager.heightOffset, manager.chosenPoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetWithHeight, step);
    }

    public void Shockwave()
    {
        gameObject.tag = "Stunned Rocky";
        Instantiate(manager.shockwave, manager.gameObject.transform.position, manager.gameObject.transform.rotation, manager.gameObject.transform);
    }

    public IEnumerator RepeatCannonball(float delay)
    {
        float timer = 0;
        while (timer < delay)
        {
            if (!manager.grabbed)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        manager.cannonball = true;
        manager.canPerformAction = true;
    }
    #endregion
}
