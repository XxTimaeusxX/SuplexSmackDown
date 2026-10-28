using UnityEngine;

public class HexagonMovement : MonoBehaviour
{
    public RockyRhodesAttacks attacks;
    public bool moveUp;
    public bool moveDown;
    public float speed;
    public float risingTime;
    public float loweringTime;

    public Material glowMaterial;
    public Material normalMaterial;

    private void Start()
    {
        risingTime = 1f;
        loweringTime = 1f;
    }

    private void Update()
    {
        if (moveUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            risingTime -= Time.deltaTime;
        }
        if (risingTime <= 0)
        {
            risingTime = 1f;
            moveUp = false;
            moveDown = true;
        }
        if (moveDown)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            loweringTime -= Time.deltaTime;
        }
        if (loweringTime <= 0)
        {
            loweringTime = 1f;
            moveDown = false;
        }
        AddGlow();
    }

    private void AddGlow()
    {
        if (attacks.chosenPoint == this.gameObject.transform)
        {
            GetComponent<Renderer>().material = glowMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = normalMaterial;
        }
    }
}
