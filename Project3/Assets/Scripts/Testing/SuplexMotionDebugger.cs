using UnityEngine;

[ExecuteAlways]
public class SuplexMotionDebugger : MonoBehaviour
{
    [Header("References")]
    public SuplexConfig suplexConfig;
    public MovementController movementController;

    [Header("Debugger Settings")]
    public SuplexAbilities suplexToPreview = SuplexAbilities.Long;
    public int resolution = 60;          // Number of points drawn
    public float gizmoSize = 0.05f;
    public Color pathColor = Color.cyan;
    public Color forwardColor = Color.yellow;
    public Color verticalColor = Color.magenta;

    [Header("Debug Options")]
    public bool showDebug = true;
    public bool drawForwardVectors = true;
    public bool drawVerticalVectors = true;
    public bool drawTrajectory = true;


    private void OnDrawGizmos()
    {
        if (suplexConfig == null || movementController == null)
            return;

        SuplexData data = suplexConfig.suplexes.Find(s => s.ability == suplexToPreview);
        if (data == null)
        {
            Debug.LogWarning($"No SuplexData found for {suplexToPreview}");
            return;
        }

        // Lock forward direction exactly like SuplexRoutine
        Vector3 forward = movementController.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 startPos = transform.position;
        Vector3 prevPos = startPos;

        Gizmos.color = pathColor;

        for (int i = 1; i <= resolution; i++)
        {
            float tNorm = (float)i / resolution;
            float t = tNorm * data.duration;

            float vY = data.verticalCurve.Evaluate(tNorm);
            float vF = data.forwardCurve.Evaluate(tNorm);

            // Calculate delta exactly like SuplexRoutine
            Vector3 delta =
                forward * vF * (data.duration / resolution) +
                Vector3.up * vY * (data.duration / resolution);

            Vector3 newPos = prevPos + delta;

            if (drawTrajectory && showDebug)
            {
                // Draw path
                Gizmos.color = pathColor;
                Gizmos.DrawLine(prevPos, newPos);
                Gizmos.DrawSphere(newPos, gizmoSize);

            }

            if (drawForwardVectors && showDebug)
            {
                // Draw forward velocity vector
                Gizmos.color = forwardColor;
                Gizmos.DrawLine(newPos, newPos + forward * (vF * 0.05f));
            }

            if (drawVerticalVectors && showDebug)
            {
                // Draw vertical velocity vector
                Gizmos.color = verticalColor;
                Gizmos.DrawLine(newPos, newPos + Vector3.up * (vY * 0.05f));
            }
            prevPos = newPos;
        }
    }
}