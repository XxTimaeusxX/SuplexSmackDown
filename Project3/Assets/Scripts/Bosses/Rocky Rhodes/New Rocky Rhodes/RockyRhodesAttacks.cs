using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class RockyRhodesAttacks : MonoBehaviour
{
    RockyRhodesManager manager;
    public Transform chosenPoint;
    public bool collided;
    RhockyAbilities abilities;

    public GameObject hitBox;

    public bool canChooseRandom;

    private float heightOffset = 2f;

    private void Awake()
    {
        manager = GetComponent<RockyRhodesManager>();
        abilities = GetComponent<RhockyAbilities>();
    }

    #region Rope Rush
    public void StartRopeRush()
    {
        if (manager.ropeRush && manager.canPerformAction)
        {
            ReadyRopeRush();
        }
        if (gameObject.CompareTag("Stunned Rocky") && manager.arena1)
        {
            StartCoroutine(RestRopeRush(4f));
        }
    }

    private IEnumerator RestRopeRush(float delay)
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
        gameObject.tag = "Rocky Rhodes";
        manager.canPerformAction = true;
        manager.numberOfRopeRusheCharges = 3;
        abilities.CheckState(RockyRhodesStates.Idle);
    }

    private void ReadyRopeRush()
    {
        manager.canPerformAction = false;
        ChooseRandomRopeRushPoint();
        Vector3 direction = (chosenPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + direction * (Vector3.Distance(transform.position, chosenPoint.position) * manager.arena1MoveSpeed);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(chosenPoint.position.x, transform.position.y, chosenPoint.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(MoveToRopeRushPoint());
    }

    private IEnumerator MoveToRopeRushPoint()
    {
        while (Vector3.Distance(transform.position, chosenPoint.position) > manager.interactionDistance)
        {
            float step = manager.arena1MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, chosenPoint.position, step);
            yield return null;
        }
        RopeRush();
    }

    private void RopeRush()
    {
        hitBox.SetActive(true);
        collided = false;
        Vector3 directionToPlayer = (manager.player.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, manager.player.transform.position) * manager.ropeRushForce);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(manager.player.transform.position.x, transform.position.y, manager.player.transform.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(RopeRushCoroutine(targetPosition));
    }

    private IEnumerator RopeRushCoroutine(Vector3 target)
    {
        while (!collided)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, manager.ropeRushForce * Time.deltaTime);
            yield return null;
        }
    }

    private void ChooseRandomRopeRushPoint()
    {
        manager.ropeRush = false;
        if (manager.ropeRushStartPoints == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.ropeRushStartPoints.Length);
        chosenPoint = manager.ropeRushStartPoints[randomPoint];
    }
    #endregion

    #region Cannonball
    public void StartCannonball()
    {
        if (manager.cannonball && manager.canPerformAction && !manager.grabbed)
        {
            gameObject.tag = "Rocky Rhodes";
            Jump();
            if (canChooseRandom)
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
        canChooseRandom = false;
        if (manager.tiles == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.tiles.Length);
        chosenPoint = manager.tiles[randomPoint];
    }

    private void SlamDown()
    {
        manager.cannonball = false;
        float step = manager.slamForce * Time.deltaTime;
        Vector3 targetWithHeight = new Vector3(chosenPoint.position.x, chosenPoint.position.y + heightOffset, chosenPoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetWithHeight, step);
    }

    private void Shockwave()
    {
        gameObject.tag = "Stunned Rocky";
        Instantiate(manager.shockwave, manager.gameObject.transform.position, manager.gameObject.transform.rotation, manager.gameObject.transform);
    }

    private IEnumerator RepeatCannonball(float delay)
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

    #region Enhanced Rope Rush
    public void StartEnhancedRopeRush()
    {
        if (manager.enhancedRopeRush && manager.canPerformAction)
        {
            ReadyEnhancedRopeRush();
        }
        if (gameObject.CompareTag("Stunned Rocky") && manager.arena3)
        {
            StartCoroutine(RestEnhancedRopeRush(4f));
        }
    }

    private IEnumerator RestEnhancedRopeRush(float delay)
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
        gameObject.tag = "Rocky Rhodes";
        manager.canPerformAction = true;
        manager.numberOfEnhancedRopeRusheCharges = 10;
        abilities.CheckState(RockyRhodesStates.Idle);
    }

    private void ReadyEnhancedRopeRush()
    {
        manager.canPerformAction = false;
        ChooseRandomEnhancedRopeRushPoint();
        Vector3 direction = (chosenPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + direction * (Vector3.Distance(transform.position, chosenPoint.position) * manager.arena3MoveSpeed);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(chosenPoint.position.x, transform.position.y, chosenPoint.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(MoveToEnhancedRopeRushPoint());
    }

    private IEnumerator MoveToEnhancedRopeRushPoint()
    {
        while (Vector3.Distance(transform.position, chosenPoint.position) > manager.interactionDistance)
        {
            float step = manager.arena3MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, chosenPoint.position, step);
            yield return null;
        }
        EnhancedRopeRush();
    }

    private void EnhancedRopeRush()
    {
        hitBox.SetActive(true);
        collided = false;
        Vector3 directionToPlayer = (manager.player.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, manager.player.transform.position) * manager.enhancedRopeRushForce);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(manager.player.transform.position.x, transform.position.y, manager.player.transform.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(EnhancedRopeRushCoroutine(targetPosition));
    }

    private IEnumerator EnhancedRopeRushCoroutine(Vector3 target)
    {
        while (!collided)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, manager.enhancedRopeRushForce * Time.deltaTime);
            yield return null;
        }
    }

    private void ChooseRandomEnhancedRopeRushPoint()
    {
        manager.enhancedRopeRush = false;
        if (manager.enhancedRopeRushStartPoints == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.enhancedRopeRushStartPoints.Length);
        chosenPoint = manager.enhancedRopeRushStartPoints[randomPoint];
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rope")) {
            manager.rb.linearVelocity = Vector3.zero;
            collided = true;
            gameObject.tag = "Stunned Rocky";
            hitBox.SetActive(false);
            if (manager.arena3 && manager.numberOfEnhancedRopeRusheCharges > 0)
            {
                gameObject.tag = "Rocky Rhodes";
                manager.numberOfEnhancedRopeRusheCharges -= 1;
                manager.canPerformAction = true;
                manager.enhancedRopeRush = true;
            }
            if (manager.arena1 && manager.numberOfRopeRusheCharges > 0)
            {
                gameObject.tag = "Rocky Rhodes";
                manager.numberOfRopeRusheCharges -= 1;
                manager.canPerformAction = true;
                manager.ropeRush = true;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Arena2"))
        {
            canChooseRandom = true;
            manager.agent.enabled = true;
            if (manager.canPerformAction && !manager.grabbed)
            {
                manager.canPerformAction = false;
                Shockwave();
                manager.rb.linearVelocity = Vector3.zero;
                gameObject.tag = "Stunned Rocky";
                if (manager.arena2 || manager.arena1)
                {
                    StartCoroutine(RepeatCannonball(manager.jumpDelay));
                }
            }
        }
    }
}