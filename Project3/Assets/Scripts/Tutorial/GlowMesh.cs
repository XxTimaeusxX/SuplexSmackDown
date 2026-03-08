using UnityEngine;

public class GlowMesh : MonoBehaviour
{
    public Material glowMaterial;
    private Renderer[] _CollectRenderers;
    void Start()
    {
        _CollectRenderers = GetComponentsInChildren<Renderer>();
     //   SetGlowColor();
     //   Debug.Log("GlowMesh material set to glowMaterial");
    }

    public void SetGlowColor()
    {
        foreach (Renderer renderer in _CollectRenderers) { renderer.material = glowMaterial; }
       //     Debug.Log($"Material replaced on {_CollectRenderers.Length} renderers.");  
    }

}
