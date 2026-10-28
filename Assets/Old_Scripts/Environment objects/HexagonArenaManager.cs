using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class HexagonArenaManager : MonoBehaviour
{
    public List<GameObject> hexagons;
    HexagonMovement movement;
    public float maxTimeToRepeat;
    public float timeToRepeat;
    public float platformSpeed;
    public float movementTime;

    private void Start()
    {
        timeToRepeat = maxTimeToRepeat;
    }

    private void Update()
    {
        timeToRepeat -= Time.deltaTime;
        if (timeToRepeat <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                ChooseRandomPlatform();
            }
            timeToRepeat = maxTimeToRepeat;
        }
    }

    private void ChooseRandomPlatform()
    {
        int randomIndex = Random.Range(0, hexagons.Count);
        GameObject chosenObject = hexagons[randomIndex];
        movement = chosenObject.GetComponent<HexagonMovement>();
        if (!movement.moveUp && !movement.moveDown)
        {
            movement.speed = platformSpeed;
            movement.moveUp = true;
            movement.risingTime = movementTime;
            movement.loweringTime = movementTime;
        }
        else
        {
            ChooseRandomPlatform();
        }
    }
}
