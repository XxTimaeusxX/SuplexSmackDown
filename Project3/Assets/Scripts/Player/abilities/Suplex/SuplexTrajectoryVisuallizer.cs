using UnityEngine;

public class SuplexTrajectoryVisualizer : MonoBehaviour
{
    [Header("Trajectory Rendering")]
    public LineRenderer trajectoryRenderer;
    [SerializeField] private int trajectorySteps = 60;

    [Header("Trajectory Materials")]
    public Material longSuplexMaterial;
    public Material rainbowSuplexMaterial;
    public Material superSuplexMaterial;

    [Header("Landing Marker Prefabs")]
    public GameObject RainbowArcLandPrefab;
    public GameObject LongArcLandPrefab;
    public GameObject SuperLandPrefab;
    [SerializeField] private Vector3 targetLandOffset = new Vector3(0f, 0.02f, 0f);

    [Header("Landing Icon Colors")]
    public Color longIconColor = new Color(1f, 0.15f, 0.15f, 1f);
    public Color rainbowIconColor = new Color(0.25f, 0.5f, 1f, 1f);
    public Color superIconColor = new Color(1f, 0.92f, 0.2f, 1f);

    private GameObject _targetLandInstance;
    private SpriteRenderer _targetLandSpriteRenderer;
    private SuplexAbilities _landingMarkerAbility = SuplexAbilities.None;

    private Transform playerTransform;

    public void Initialize(Transform heldEnemyTransform)
    {
        playerTransform = heldEnemyTransform;
    }

    public void ShowTrajectory(SuplexData data)
    {
        if (trajectoryRenderer == null || playerTransform == null)
            return;

        Vector3[] points = new Vector3[trajectorySteps + 1];

        // Match SuplexMotionDebugger forward lock
        Vector3 forward = playerTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 startPos = playerTransform.position;
        Vector3 prevPos = startPos;

        float dt = data.duration / trajectorySteps;

        points[0] = startPos;
        int lastPoint = 0;
        bool hitDetected = false;
        Vector3 landingNormal = Vector3.up;

        for (int i = 1; i <= trajectorySteps; i++)
        {
            float tNorm = (float)i / trajectorySteps;

            float vY = data.verticalCurve.Evaluate(tNorm);
            float vF = data.forwardCurve.Evaluate(tNorm);

            Vector3 delta =
                forward * vF * dt +
                Vector3.up * vY * dt;

            Vector3 newPos = prevPos + delta;

            // Landing detection
            if (Physics.Linecast(prevPos, newPos, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
            {
                points[i] = hit.point;
                landingNormal = hit.normal;
                lastPoint = i;
                hitDetected = true;
                break;
            }

            points[i] = newPos;
            prevPos = newPos;
            lastPoint = i;
        }

        // Render the curve
        trajectoryRenderer.enabled = true;
        trajectoryRenderer.positionCount = lastPoint + 1;
        for (int i = 0; i <= lastPoint; i++)
            trajectoryRenderer.SetPosition(i, points[i]);

        // Handle landing marker
        if (hitDetected)
        {
            UpdateTargetLand(points[lastPoint], landingNormal);
        }
        else
        {
            // No landing > hide marker
            SetTargetLandActive(false);
        }
    }

    public void SetTrajectoryMaterial(SuplexAbilities ability)
    {
        Material rendererMaterial = null;
        Color iconColor = Color.white;
        GameObject prefab = null;

        switch (ability)
        {
            case SuplexAbilities.Long:
                rendererMaterial = longSuplexMaterial;
                iconColor = longIconColor;
                prefab = LongArcLandPrefab;
                break;
            case SuplexAbilities.Rainbow:
                rendererMaterial = rainbowSuplexMaterial;
                iconColor = rainbowIconColor;
                prefab = RainbowArcLandPrefab;
                break;
            case SuplexAbilities.Super:
                rendererMaterial = superSuplexMaterial;
                iconColor = superIconColor;
                prefab = SuperLandPrefab;
                break;
        }

        if (trajectoryRenderer != null && rendererMaterial != null)
            trajectoryRenderer.material = rendererMaterial;

        if (prefab == null)
        {
            if (_targetLandInstance != null)
            {
                Destroy(_targetLandInstance);
                _targetLandInstance = null;
                _targetLandSpriteRenderer = null;
                _landingMarkerAbility = SuplexAbilities.None;
            }
            return;
        }

        if (_targetLandInstance == null || _landingMarkerAbility != ability)
        {
            if (_targetLandInstance != null) Destroy(_targetLandInstance);
            _targetLandInstance = Instantiate(prefab);
            _targetLandInstance.SetActive(false);
            _targetLandSpriteRenderer = _targetLandInstance.GetComponentInChildren<SpriteRenderer>();
            _landingMarkerAbility = ability;
        }

        if (_targetLandSpriteRenderer != null)
            _targetLandSpriteRenderer.color = iconColor;
    }

    private void UpdateTargetLand(Vector3 position, Vector3 normal)
    {
        if (_targetLandInstance == null) return;

        var t = _targetLandInstance.transform;
        t.position = position + targetLandOffset;
        t.rotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (!_targetLandInstance.activeSelf)
            _targetLandInstance.SetActive(true);
    }

    public void SetTargetLandActive(bool active)
    {
        if (_targetLandInstance == null) return;
        if (_targetLandInstance.activeSelf != active)
            _targetLandInstance.SetActive(active);
    }

    public void ClearTrajectory()
    {
        if (trajectoryRenderer != null)
            trajectoryRenderer.positionCount = 0;
    }

    private void OnDestroy()
    {
        if (_targetLandInstance != null)
            Destroy(_targetLandInstance);
    }
}