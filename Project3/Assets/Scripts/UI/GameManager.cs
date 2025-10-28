using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    public Slider enemyHealthSlider;
    public int enemySharedHealth;
    public int enemyHealthdamage;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}