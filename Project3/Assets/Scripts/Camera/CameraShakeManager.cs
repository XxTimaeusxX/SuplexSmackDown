using System.Xml.Serialization;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;
    [SerializeField] private float GlobalShakeForce = 1f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void SuplexCameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulse(GlobalShakeForce * 1.5f);
    }
}
