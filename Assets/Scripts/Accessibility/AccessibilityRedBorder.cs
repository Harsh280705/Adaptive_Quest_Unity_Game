using UnityEngine;

public class AccessibilityRedBorder : MonoBehaviour
{
    [Header("RED ACCESSIBILITY BORDER")]
    [SerializeField]
    private Color borderColor = new Color(1f, 0f, 0f, 1f);

    [SerializeField]
    private float borderWidth = 0.08f;

    [SerializeField]
    private float borderPadding = 0.08f;

    [SerializeField]
    private float pulseSpeed = 4f;

    [SerializeField]
    private float pulseAmount = 0.025f;

    private LineRenderer[] borderLines;

    private Renderer[] targetRenderers;

    private bool highlighted;

    private Material borderMaterial;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        FindRenderers();
    }


    // =========================================================
    // FIND ALL RENDERERS
    // =========================================================

    private void FindRenderers()
    {
        targetRenderers =
            GetComponentsInChildren<Renderer>(true);
    }


    // =========================================================
    // RUNTIME HIGHLIGHT
    // =========================================================

    public void SetRuntimeHighlight(bool enabled)
    {
        highlighted = enabled;

        if (enabled)
        {
            if (targetRenderers == null ||
                targetRenderers.Length == 0)
            {
                FindRenderers();
            }

            CreateBorder();

            UpdateBorder();

            SetBorderVisible(true);
        }
        else
        {
            SetBorderVisible(false);
        }
    }


    // =========================================================
    // CREATE BORDER
    // =========================================================

    private void CreateBorder()
    {
        if (borderLines != null &&
            borderLines.Length == 12)
        {
            return;
        }

        borderLines =
            new LineRenderer[12];

        for (int i = 0; i < 12; i++)
        {
            GameObject lineObject =
                new GameObject(
                    "AccessibilityRedBorder_" + i
                );

            lineObject.transform.SetParent(
                transform,
                true
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            line.useWorldSpace = true;

            line.positionCount = 2;

            line.startWidth =
                borderWidth;

            line.endWidth =
                borderWidth;

            line.numCapVertices = 4;

            line.numCornerVertices = 4;

            line.startColor =
                borderColor;

            line.endColor =
                borderColor;

            line.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;

            line.receiveShadows = false;

            line.material =
                GetBorderMaterial();

            borderLines[i] = line;
        }
    }


    // =========================================================
    // BORDER MATERIAL
    // =========================================================

    private Material GetBorderMaterial()
    {
        if (borderMaterial != null)
            return borderMaterial;

        Shader shader =
            Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shader =
                Shader.Find("Unlit/Color");
        }

        if (shader != null)
        {
            borderMaterial =
                new Material(shader);

            borderMaterial.color =
                borderColor;
        }

        return borderMaterial;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void LateUpdate()
    {
        if (!highlighted)
            return;

        if (borderLines == null)
            return;

        if (targetRenderers == null ||
            targetRenderers.Length == 0)
        {
            FindRenderers();
        }

        UpdateBorder();

        // -----------------------------------------------------
        // SUBTLE PULSE
        // -----------------------------------------------------

        float pulse =
            Mathf.Sin(
                Time.unscaledTime *
                pulseSpeed
            ) * pulseAmount;

        float width =
            borderWidth + pulse;

        for (int i = 0;
             i < borderLines.Length;
             i++)
        {
            if (borderLines[i] == null)
                continue;

            borderLines[i].startWidth = width;
            borderLines[i].endWidth = width;
        }
    }


    // =========================================================
    // CALCULATE COMBINED BOUNDS
    // =========================================================

    private Bounds GetCombinedBounds()
    {
        bool foundRenderer = false;

        Bounds combined =
            new Bounds(
                transform.position,
                Vector3.zero
            );

        for (
            int i = 0;
            i < targetRenderers.Length;
            i++
        )
        {
            Renderer renderer =
                targetRenderers[i];

            if (renderer == null)
                continue;

            if (!renderer.enabled)
                continue;

            if (!foundRenderer)
            {
                combined =
                    renderer.bounds;

                foundRenderer = true;
            }
            else
            {
                combined.Encapsulate(
                    renderer.bounds
                );
            }
        }

        if (!foundRenderer)
        {
            combined =
                new Bounds(
                    transform.position,
                    Vector3.one
                );
        }

        return combined;
    }


    // =========================================================
    // UPDATE BORDER GEOMETRY
    // =========================================================

    private void UpdateBorder()
    {
        if (borderLines == null ||
            borderLines.Length != 12)
        {
            return;
        }

        Bounds bounds =
            GetCombinedBounds();

        Vector3 min =
            bounds.min -
            Vector3.one * borderPadding;

        Vector3 max =
            bounds.max +
            Vector3.one * borderPadding;


        // -----------------------------------------------------
        // 8 CORNERS
        // -----------------------------------------------------

        Vector3 c0 =
            new Vector3(
                min.x,
                min.y,
                min.z
            );

        Vector3 c1 =
            new Vector3(
                max.x,
                min.y,
                min.z
            );

        Vector3 c2 =
            new Vector3(
                max.x,
                max.y,
                min.z
            );

        Vector3 c3 =
            new Vector3(
                min.x,
                max.y,
                min.z
            );

        Vector3 c4 =
            new Vector3(
                min.x,
                min.y,
                max.z
            );

        Vector3 c5 =
            new Vector3(
                max.x,
                min.y,
                max.z
            );

        Vector3 c6 =
            new Vector3(
                max.x,
                max.y,
                max.z
            );

        Vector3 c7 =
            new Vector3(
                min.x,
                max.y,
                max.z
            );


        // -----------------------------------------------------
        // BOTTOM
        // -----------------------------------------------------

        SetLine(
            0,
            c0,
            c1
        );

        SetLine(
            1,
            c1,
            c2
        );

        SetLine(
            2,
            c2,
            c3
        );

        SetLine(
            3,
            c3,
            c0
        );


        // -----------------------------------------------------
        // TOP
        // -----------------------------------------------------

        SetLine(
            4,
            c4,
            c5
        );

        SetLine(
            5,
            c5,
            c6
        );

        SetLine(
            6,
            c6,
            c7
        );

        SetLine(
            7,
            c7,
            c4
        );


        // -----------------------------------------------------
        // VERTICAL
        // -----------------------------------------------------

        SetLine(
            8,
            c0,
            c4
        );

        SetLine(
            9,
            c1,
            c5
        );

        SetLine(
            10,
            c2,
            c6
        );

        SetLine(
            11,
            c3,
            c7
        );
    }


    // =========================================================
    // SET LINE
    // =========================================================

    private void SetLine(
        int index,
        Vector3 start,
        Vector3 end
    )
    {
        if (borderLines[index] == null)
            return;

        borderLines[index]
            .SetPosition(0, start);

        borderLines[index]
            .SetPosition(1, end);
    }


    // =========================================================
    // VISIBILITY
    // =========================================================

    private void SetBorderVisible(
        bool visible
    )
    {
        if (borderLines == null)
            return;

        for (
            int i = 0;
            i < borderLines.Length;
            i++
        )
        {
            if (borderLines[i] != null)
            {
                borderLines[i].enabled =
                    visible;
            }
        }
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (borderMaterial != null)
        {
            Destroy(borderMaterial);
        }
    }
}