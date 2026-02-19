
using UnityEngine;
using UnityEngine.UI;

public class LowerRoom : MonoBehaviour
{
    float speed = 8.0f;
    public Vector3 TargetDistance;
    public CanvasGroup FloorArrowCanvas;
    public CanvasGroup TargetArrowCanvas;
 
    void Start()
    {
        FloorArrowCanvas.alpha = 0;
        TargetArrowCanvas.alpha = 0;
        TargetDistance = transform.position + Vector3.down * 4.0f - Vector3.right * 30.0f;

    }
    public void EnableArrows()
    {
        FloorArrowCanvas.alpha = 1;
        TargetArrowCanvas.alpha = 1;
    }
    public void MoveDown()
    {
       
        transform.position = Vector3.MoveTowards(transform.position, TargetDistance, speed * Time.deltaTime);
    }
}
