
using System.Collections;
using UnityEngine;
// ~ Ovi. 



// TODO: spirit dancer: increase attack projectile size, movement speed and projectile speed when colliding with Marigold
//TODO: make ghost wrestler boss invunerable for a short time after colliding with Marigold, and play a unique animation to show it.
//TODO: 
public class MariGold : MonoBehaviour
{
    public float jumpforce = 10f;
    private bool _isSpinning = false;
    private float _speed = 5f; // Speed of floating
    private float _height = 1f; // Height of floating
    private Vector3 _StartPos;

    public Material highlightMaterial;
   private Renderer[] _MariGoldrenderers;
    private Material[] _originalMaterials;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _StartPos = transform.position;
        _MariGoldrenderers = GetComponentsInChildren<Renderer>();
        // Store original materials (one per renderer)
        _originalMaterials = new Material[_MariGoldrenderers.Length];
        for (int i = 0; i < _MariGoldrenderers.Length; i++)
        {
            _originalMaterials[i] = _MariGoldrenderers[i].material;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        Floating();
    }
    public void Floating ()
    {
        float newY = _StartPos.y + Mathf.Sin(Time.time * _speed) * _height;
        transform.position = new Vector3(_StartPos.x, newY, _StartPos.z);
    }

    public IEnumerator SpinandHighlight()
    {  if(_isSpinning) yield break; // Prevent multiple simultaneous spins
        _isSpinning = true;
        foreach (Renderer renderer in _MariGoldrenderers) { renderer.material = highlightMaterial; }
        float spintimer = 0f;
        while(spintimer< 5f)
        {
            transform.Rotate(Vector3.up * 900 * Time.deltaTime);
            spintimer += Time.deltaTime;
            yield return null;
        }

        // Reset to original materials after spin completes
        for (int i = 0; i < _MariGoldrenderers.Length; i++)
        {
            _MariGoldrenderers[i].material = _originalMaterials[i];
        }
        _isSpinning = false;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.velocity = transform.up* jumpforce;
                StartCoroutine(SpinandHighlight());
              //  Debug.Log("Player hit MariGold, applying jump force.");
            }
        }
    }
}
