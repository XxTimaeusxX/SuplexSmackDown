using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores configuration for each type of suplex (height, distance, speed, etc). 
/// </summary>
public class SuplexConfig : MonoBehaviour
{
    public Transform carryPoint;      // Where the grabbed enemy is held

    public GameObject shockwave;
    public GameObject rageShockwave;

    public Slider rageBar;

    public Image suplexBar;
    public Sprite suplexImg1;

    public CinemachineImpulseSource impulseSource;

    public AnimationCurve CameraOffsetCurve; // line graph to control camera offset during suplex

    public float longSuplexBuffer = 0.8f;

    public List<SuplexData> suplexes;   // assign in inspector
    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
}
