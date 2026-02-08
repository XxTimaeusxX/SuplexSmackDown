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
     public Color longIconColor = new Color(1f, 0.15f, 0.15f, 1f);//red 
     public Color rainbowIconColor = new Color(0.25f, 0.5f, 1f, 1f); // Blue
     public Color superIconColor = new Color(1f, 0.92f, 0.2f, 1f);  //yellow

    // Runtime-only references
    private GameObject _targetLandInstance;
    private SpriteRenderer _targetLandSpriteRenderer;
    private SuplexAbilities _landingMarkerAbility = SuplexAbilities.None;

    // Context needed for trajectory calculation
    private PlayerMovement _playerMovement;
    private Transform _heldEnemyTransform;
    private float _currentGravityScale = 1f;

    /*
    public void Initialize(PlayerMovement playerMovement, Transform heldEnemyTransform) //initialize references caching
    {
        _playerMovement = playerMovement;
        _heldEnemyTransform = heldEnemyTransform;
    }

    public void SetGravityScale(float gravityScale)
    {
        _currentGravityScale = gravityScale;
    }

    public void ShowTrajectory(SuplexConfig config)
    {
        if (trajectoryRenderer == null || _heldEnemyTransform == null || _playerMovement == null)
            return;

        Vector3[] points = new Vector3[trajectorySteps + 1];
        Vector3 startPos = _heldEnemyTransform.position;
        float gravity = Mathf.Abs(_playerMovement.gravity) * _currentGravityScale;

        float height = config.LiftHeight;
        float distance = config.FowardDistance;
        float minTimeToPeak = Mathf.Sqrt(2f * height / gravity);
        float minTotalTime = minTimeToPeak * 2f;
        float totalTime = Mathf.Max(config.LaunchSpeed, minTotalTime);
        float timeToPeak = totalTime / 2f;
        float vy = (2f * height) / timeToPeak;
        float vx = distance / totalTime;

        Vector3 forward = _heldEnemyTransform.forward.normalized;
        Vector3 launchVelocity = forward * vx + Vector3.up * vy;

        Vector3 pos = startPos;
        Vector3 velocity = launchVelocity;
        float dt = totalTime / trajectorySteps;
        int lastPoint = 0;

        trajectoryRenderer.alignment = LineAlignment.View;
        trajectoryRenderer.enabled = true;

        // Disable enemy colliders to prevent interference with trajectory calculation
        Collider[] enemyColliders = _heldEnemyTransform.GetComponentsInChildren<Collider>();
        foreach (var col in enemyColliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // Track landing (position + normal) for the marker
        Vector3 landingNormal = Vector3.up;
        points[0] = pos;

        for (int i = 1; i <= trajectorySteps; i++)
        {
            // Apply gravity
            velocity += Vector3.down * gravity * dt;
            Vector3 nextPos = pos + velocity * dt;

            // Ignore triggers to reduce false hits
            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
            {
                points[i] = hit.point;
                landingNormal = hit.normal;
                lastPoint = i;
                break;
            }

            if (nextPos.y <= startPos.y)
            {
                nextPos.y = startPos.y;
                points[i] = nextPos;

                // Try to get a ground normal under the clamped point
                if (Physics.Raycast(nextPos + Vector3.up * 0.25f, Vector3.down, out RaycastHit groundHit, 5f, ~0, QueryTriggerInteraction.Ignore))
                    landingNormal = groundHit.normal;
                lastPoint = i;
                break;
            }

            points[i] = nextPos;
            pos = nextPos;
            lastPoint = i;
        }

        // Re-enable enemy colliders
        foreach (var col in enemyColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        trajectoryRenderer.positionCount = lastPoint + 1;
        for (int i = 0; i <= lastPoint; i++)
            trajectoryRenderer.SetPosition(i, points[i]);

        // Place/update landing marker
        Vector3 landingPos = points[lastPoint];
        UpdateTargetLand(landingPos, landingNormal);
    }

    public void SetTrajectoryMaterial(SuplexAbilities ability)
    {
        // Pick trajectory material, icon color, and prefab in one place
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
            default:
                rendererMaterial = null;
                prefab = null;
                iconColor = Color.white;
                break;
        }

        // Apply trajectory material if available; otherwise leave existing material
        if (trajectoryRenderer != null && rendererMaterial != null)
        {
            trajectoryRenderer.material = rendererMaterial;
        }

        // If no prefab assigned for this ability, destroy any existing marker and exit
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

        // Ensure/Swap landing marker instance for this ability
        if (_targetLandInstance == null || _landingMarkerAbility != ability)
        {
            if (_targetLandInstance != null) Destroy(_targetLandInstance);
            _targetLandInstance = Instantiate(prefab);
            if (_targetLandInstance != null)
            {
                _targetLandInstance.SetActive(false);
                _targetLandSpriteRenderer = _targetLandInstance.GetComponentInChildren<SpriteRenderer>();
                _landingMarkerAbility = ability;
            }
            else
            {
                _targetLandSpriteRenderer = null;
                _landingMarkerAbility = SuplexAbilities.None;
            }
        }

        // Tint icon if available
        if (_targetLandSpriteRenderer != null)
            _targetLandSpriteRenderer.color = iconColor;
    }

    private void UpdateTargetLand(Vector3 position, Vector3 normal)
    {
        if (_targetLandInstance == null) return;

        var t = _targetLandInstance.transform;
        t.position = position + targetLandOffset;
        t.rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        if (!_targetLandInstance.activeSelf) _targetLandInstance.SetActive(true);
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
    */
}