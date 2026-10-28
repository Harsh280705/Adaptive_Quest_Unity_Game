using UnityEngine;
using UnityEngine.Rendering;

public class AccessibilityVisualAssist : MonoBehaviour
{
    [Header("VISUAL ASSISTANCE HIGHLIGHT")]

    [SerializeField]
    private Color highlightColor =
        new Color(1f, 0.85f, 0.05f, 1f);

    [SerializeField]
    private float outlineWidth = 0.08f;

    [SerializeField]
    private float glowWidth = 0.18f;

    [SerializeField]
    private float boundsPadding = 0.08f;

    [SerializeField]
    private bool pulse = true;

    [SerializeField]
    private float pulseSpeed = 4f;


    private Renderer targetRenderer;

    private GameObject outlineObject;
    private LineRenderer outline;

    private GameObject glowObject;
    private LineRenderer glow;

    private bool runtimeHighlighted;
    private bool initialized;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        targetRenderer =
            GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            targetRenderer =
                GetComponentInChildren<Renderer>();
        }

        initialized =
            targetRenderer != null;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        /*
         * IMPORTANT:
         *
         * Do NOT automatically highlight every object.
         *
         * AccessibilityRuntime decides which object is
         * currently important.
         */
        SetRuntimeHighlight(false);
    }


    // =========================================================
    // PUBLIC HIGHLIGHT CONTROL
    // =========================================================

    public void SetRuntimeHighlight(
        bool highlighted)
    {
        runtimeHighlighted =
            highlighted;

        if (!runtimeHighlighted)
        {
            RemoveHighlight();
            return;
        }

        if (!initialized)
            return;

        CreateHighlight();

        UpdateHighlight();
    }


    // =========================================================
    // CREATE
    // =========================================================

    private void CreateHighlight()
    {
        // -----------------------------------------------------
        // MAIN OUTLINE
        // -----------------------------------------------------

        if (outlineObject == null)
        {
            outlineObject =
                new GameObject(
                    "Accessibility_Target_Outline"
                );

            outlineObject.transform.SetParent(
                transform,
                false
            );

            outline =
                outlineObject.AddComponent<LineRenderer>();

            ConfigureLine(
                outline,
                outlineWidth,
                1f
            );
        }


        // -----------------------------------------------------
        // GLOW
        // -----------------------------------------------------

        if (glowObject == null)
        {
            glowObject =
                new GameObject(
                    "Accessibility_Target_Glow"
                );

            glowObject.transform.SetParent(
                transform,
                false
            );

            glow =
                glowObject.AddComponent<LineRenderer>();

            ConfigureLine(
                glow,
                glowWidth,
                0.30f
            );
        }
    }


    // =========================================================
    // CONFIGURE LINE
    // =========================================================

    private void ConfigureLine(
        LineRenderer line,
        float width,
        float alpha)
    {
        line.useWorldSpace = true;

        line.loop = false;

        line.positionCount = 24;

        line.startWidth = width;
        line.endWidth = width;

        line.shadowCastingMode =
            ShadowCastingMode.Off;

        line.receiveShadows = false;

        line.material =
            CreateMaterial();

        Color color =
            highlightColor;

        color.a =
            alpha;

        line.startColor =
            color;

        line.endColor =
            color;

        if (line.material != null)
        {
            line.material.color =
                color;

            /*
             * Render above normal geometry.
             */
            line.material.renderQueue =
                4000;

            /*
             * ZTest Always.
             *
             * The highlight remains visible even
             * when part of the outline would normally
             * be hidden by the object.
             */
            if (line.material.HasProperty("_ZTest"))
            {
                line.material.SetInt(
                    "_ZTest",
                    8
                );
            }
        }
    }


    // =========================================================
    // MATERIAL
    // =========================================================

    private Material CreateMaterial()
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
        {
            shader =
                Shader.Find(
                    "Unlit/Color"
                );
        }

        if (shader == null)
            return null;

        return new Material(shader);
    }


    // =========================================================
    // UPDATE HIGHLIGHT
    // =========================================================

    private void UpdateHighlight()
    {
        if (targetRenderer == null ||
            outline == null)
        {
            return;
        }


        Bounds bounds =
            targetRenderer.bounds;


        Vector3 padding =
            Vector3.one *
            boundsPadding;


        Vector3 min =
            bounds.min -
            padding;

        Vector3 max =
            bounds.max +
            padding;


        Vector3 p000 =
            new Vector3(
                min.x,
                min.y,
                min.z
            );

        Vector3 p001 =
            new Vector3(
                min.x,
                min.y,
                max.z
            );

        Vector3 p010 =
            new Vector3(
                min.x,
                max.y,
                min.z
            );

        Vector3 p011 =
            new Vector3(
                min.x,
                max.y,
                max.z
            );

        Vector3 p100 =
            new Vector3(
                max.x,
                min.y,
                min.z
            );

        Vector3 p101 =
            new Vector3(
                max.x,
                min.y,
                max.z
            );

        Vector3 p110 =
            new Vector3(
                max.x,
                max.y,
                min.z
            );

        Vector3 p111 =
            new Vector3(
                max.x,
                max.y,
                max.z
            );


        Vector3[] points =
        {
            // Bottom
            p000, p001,
            p001, p101,
            p101, p100,
            p100, p000,

            // Top
            p010, p011,
            p011, p111,
            p111, p110,
            p110, p010,

            // Vertical
            p000, p010,
            p001, p011,
            p100, p110,
            p101, p111
        };


        outline.positionCount =
            points.Length;

        outline.SetPositions(
            points
        );


        if (glow != null)
        {
            glow.positionCount =
                points.Length;

            glow.SetPositions(
                points
            );
        }
    }


    // =========================================================
    // LATE UPDATE
    // =========================================================

    private void LateUpdate()
    {
        if (!runtimeHighlighted)
            return;

        UpdateHighlight();


        float intensity =
            pulse
                ? 0.75f +
                  0.25f *
                  (
                      0.5f +
                      0.5f *
                      Mathf.Sin(
                          Time.unscaledTime *
                          pulseSpeed
                      )
                  )
                : 1f;


        if (outline != null)
        {
            Color color =
                highlightColor;

            color.a =
                intensity;

            outline.startColor =
                color;

            outline.endColor =
                color;
        }


        if (glow != null)
        {
            Color color =
                highlightColor;

            color.a =
                0.18f +
                0.18f *
                intensity;

            glow.startColor =
                color;

            glow.endColor =
                color;
        }
    }


    // =========================================================
    // REMOVE
    // =========================================================

    private void RemoveHighlight()
    {
        if (outlineObject != null)
        {
            Destroy(
                outlineObject
            );
        }

        if (glowObject != null)
        {
            Destroy(
                glowObject
            );
        }


        outlineObject = null;
        glowObject = null;

        outline = null;
        glow = null;
    }
}