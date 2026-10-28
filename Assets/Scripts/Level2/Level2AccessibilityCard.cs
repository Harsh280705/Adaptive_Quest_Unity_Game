using UnityEngine;

public class Level2AccessibilityCard : MonoBehaviour
{
    [Header("LARGE INTERACTION ZONE")]

    [SerializeField]
    private Vector3 largeColliderMultiplier =
        new Vector3(2.2f, 2.0f, 2.2f);

    private BoxCollider boxCollider;

    private Vector3 originalColliderSize;

    private GameObject accessibilityOutline;

    private LineRenderer outline;

    [Header("LEVEL 2 VISUAL ASSISTANCE")]

    [SerializeField]
    private Color outlineColor = Color.black;

    [SerializeField]
    private float outlineWidth = 0.045f;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        boxCollider =
            GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            originalColliderSize =
                boxCollider.size;
        }
    }

    private void Start()
    {
        ApplyAccessibility();
    }

    private void OnEnable()
    {
        AccessibilityManager.AccessibilitySettingsChanged +=
            ApplyAccessibility;
    }

    private void OnDisable()
    {
        AccessibilityManager.AccessibilitySettingsChanged -=
            ApplyAccessibility;
    }

    // =========================================================
    // APPLY
    // =========================================================

    public void ApplyAccessibility()
    {
        if (AccessibilityManager.Instance == null)
            return;

        bool largeZones =
            AccessibilityManager.Instance
                .IsLargeInteractionZonesEnabled();

        bool visualAssist =
            AccessibilityManager.Instance
                .IsVisualAssistanceEnabled();

        // -----------------------------------------------------
        // LARGE INTERACTION ZONE
        // -----------------------------------------------------

        if (boxCollider != null)
        {
            if (largeZones)
            {
                boxCollider.size =
                    Vector3.Scale(
                        originalColliderSize,
                        largeColliderMultiplier
                    );
            }
            else
            {
                boxCollider.size =
                    originalColliderSize;
            }
        }

        // -----------------------------------------------------
        // VISUAL ASSISTANCE
        // -----------------------------------------------------

        if (visualAssist)
        {
            CreateBlackOutline();
        }
        else
        {
            RemoveBlackOutline();
        }
    }

    // =========================================================
    // BLACK OUTLINE
    // =========================================================

    private void CreateBlackOutline()
    {
        if (accessibilityOutline != null)
        {
            UpdateOutline();
            return;
        }

        accessibilityOutline =
            new GameObject(
                "Accessibility_Black_Outline"
            );

        accessibilityOutline.transform.SetParent(
            transform,
            false
        );

        outline =
            accessibilityOutline.AddComponent<LineRenderer>();

        outline.useWorldSpace = false;

        outline.loop = true;

        outline.positionCount = 4;

        outline.startWidth =
            outlineWidth;

        outline.endWidth =
            outlineWidth;

        outline.startColor =
            outlineColor;

        outline.endColor =
            outlineColor;

        outline.material =
            CreateOutlineMaterial();

        outline.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        outline.receiveShadows = false;

        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (outline == null ||
            boxCollider == null)
        {
            return;
        }

        Vector3 center =
            boxCollider.center;

        Vector3 size =
            boxCollider.size;

        float x =
            size.x * 0.55f;

        float y =
            size.y * 0.55f;

        float z =
            size.z * 0.55f;

        Vector3[] points =
        {
            center + new Vector3(-x, -y, -z),
            center + new Vector3( x, -y, -z),
            center + new Vector3( x,  y, -z),
            center + new Vector3(-x,  y, -z)
        };

        outline.SetPositions(points);
    }

    // =========================================================
    // MATERIAL
    // =========================================================

    private Material CreateOutlineMaterial()
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
        {
            shader =
                Shader.Find("Unlit/Color");
        }

        Material material =
            new Material(shader);

        material.color =
            outlineColor;

        return material;
    }

    // =========================================================
    // REMOVE
    // =========================================================

    private void RemoveBlackOutline()
    {
        if (accessibilityOutline != null)
        {
            Destroy(accessibilityOutline);

            accessibilityOutline = null;
            outline = null;
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void LateUpdate()
    {
        if (accessibilityOutline != null)
        {
            UpdateOutline();
        }
    }
}