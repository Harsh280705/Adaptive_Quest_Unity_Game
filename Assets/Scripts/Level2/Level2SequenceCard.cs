using System.Collections;
using UnityEngine;

public class Level2SequenceCard : MonoBehaviour
{
    [Header("CARD")]
    [SerializeField]
    private int cardNumber;


    [Header("CARD VISUAL")]
    [SerializeField]
    private Renderer cardRenderer;


    [Header("ACCESSIBILITY BORDER")]
    [SerializeField]
    private Color accessibilityBorderColor =
        new Color(1f, 0f, 0f, 1f);


    [SerializeField]
    private float borderWidth = 0.035f;


    [SerializeField]
    private float borderOffset = 0.03f;


    [SerializeField]
    private float borderPulseSpeed = 4f;


    [SerializeField]
    private float borderPulseAmount = 0.025f;


    private Level2SequenceController controller;

    private Color originalColor;

    private Material cardMaterial;

    private bool accessibilityHighlighted;

    private LineRenderer accessibilityBorder;

    private Camera playerCamera;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (cardRenderer == null)
        {
            cardRenderer =
                GetComponent<Renderer>();
        }


        if (cardRenderer != null)
        {
            cardMaterial =
                cardRenderer.material;

            originalColor =
                cardMaterial.color;
        }


        playerCamera =
            Camera.main;


        /*
         * IMPORTANT
         *
         * Level 2 uses its own border.
         *
         * We intentionally disable AccessibilityVisualAssist
         * ONLY on this Level 2 card so it cannot calculate a
         * large border around neighbouring cards.
         *
         * Level 1 is NOT touched.
         */

        AccessibilityVisualAssist oldAssist =
            GetComponent<AccessibilityVisualAssist>();


        if (oldAssist != null)
        {
            oldAssist.enabled = false;
        }


        CreateAccessibilityBorder();

        SetBorderVisible(false);
    }


    // =========================================================
    // CREATE BORDER
    // =========================================================

    private void CreateAccessibilityBorder()
    {
        GameObject borderObject =
            new GameObject(
                "Level2_Accessibility_Red_Border"
            );


        borderObject.transform.SetParent(
            transform,
            true
        );


        accessibilityBorder =
            borderObject.AddComponent<LineRenderer>();


        accessibilityBorder.useWorldSpace =
            true;


        accessibilityBorder.loop =
            true;


        accessibilityBorder.positionCount =
            4;


        accessibilityBorder.startWidth =
            borderWidth;


        accessibilityBorder.endWidth =
            borderWidth;


        accessibilityBorder.numCornerVertices =
            4;


        accessibilityBorder.numCapVertices =
            4;


        accessibilityBorder.material =
            new Material(
                Shader.Find("Sprites/Default")
            );


        accessibilityBorder.startColor =
            accessibilityBorderColor;


        accessibilityBorder.endColor =
            accessibilityBorderColor;


        accessibilityBorder.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;


        accessibilityBorder.receiveShadows =
            false;


        accessibilityBorder.enabled =
            false;
    }


    // =========================================================
    // SETUP
    // =========================================================

    public void Setup(
        Level2SequenceController sequenceController,
        int number
    )
    {
        controller =
            sequenceController;

        cardNumber =
            number;
    }


    // =========================================================
    // CARD COLOR
    // =========================================================

    public void SetCardColor(
        Color color
    )
    {
        if (cardRenderer == null)
        {
            cardRenderer =
                GetComponent<Renderer>();
        }


        if (cardRenderer != null)
        {
            if (cardMaterial == null)
            {
                cardMaterial =
                    cardRenderer.material;
            }


            cardMaterial.color =
                color;


            originalColor =
                color;
        }
    }


    // =========================================================
    // ACCESSIBILITY HIGHLIGHT
    // =========================================================

    public void SetAccessibilityHighlight(
        bool highlighted
    )
    {
        accessibilityHighlighted =
            highlighted;


        if (highlighted)
        {
            UpdateAccessibilityBorder();

            SetBorderVisible(true);
        }
        else
        {
            SetBorderVisible(false);
        }
    }


    // =========================================================
    // SHOW / HIDE BORDER
    // =========================================================

    private void SetBorderVisible(
        bool visible
    )
    {
        if (accessibilityBorder == null)
            return;


        accessibilityBorder.enabled =
            visible;
    }


    // =========================================================
    // UPDATE BORDER
    // =========================================================

    private void UpdateAccessibilityBorder()
    {
        if (accessibilityBorder == null)
            return;


        if (cardRenderer == null)
            return;


        if (playerCamera == null)
        {
            playerCamera =
                Camera.main;
        }


        if (playerCamera == null)
            return;


        Bounds bounds =
            cardRenderer.bounds;


        Vector3 center =
            bounds.center;


        /*
         * Use the camera's right/up directions.
         *
         * This makes the border tightly surround THIS CARD
         * from the player's viewpoint instead of surrounding
         * neighbouring cards.
         */

        Vector3 cameraRight =
            playerCamera.transform.right;


        Vector3 cameraUp =
            playerCamera.transform.up;


        Vector3 cameraForward =
            playerCamera.transform.forward;


        Vector3[] corners =
            new Vector3[8];


        Vector3 extents =
            bounds.extents;


        corners[0] =
            center +
            new Vector3(
                -extents.x,
                -extents.y,
                -extents.z
            );


        corners[1] =
            center +
            new Vector3(
                extents.x,
                -extents.y,
                -extents.z
            );


        corners[2] =
            center +
            new Vector3(
                extents.x,
                extents.y,
                -extents.z
            );


        corners[3] =
            center +
            new Vector3(
                -extents.x,
                extents.y,
                -extents.z
            );


        corners[4] =
            center +
            new Vector3(
                -extents.x,
                -extents.y,
                extents.z
            );


        corners[5] =
            center +
            new Vector3(
                extents.x,
                -extents.y,
                extents.z
            );


        corners[6] =
            center +
            new Vector3(
                extents.x,
                extents.y,
                extents.z
            );


        corners[7] =
            center +
            new Vector3(
                -extents.x,
                extents.y,
                extents.z
            );


        float minRight =
            float.MaxValue;

        float maxRight =
            float.MinValue;

        float minUp =
            float.MaxValue;

        float maxUp =
            float.MinValue;

        float maxDepth =
            float.MinValue;


        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 relative =
                corners[i] -
                center;


            float right =
                Vector3.Dot(
                    relative,
                    cameraRight
                );


            float up =
                Vector3.Dot(
                    relative,
                    cameraUp
                );


            float depth =
                Vector3.Dot(
                    relative,
                    cameraForward
                );


            minRight =
                Mathf.Min(
                    minRight,
                    right
                );


            maxRight =
                Mathf.Max(
                    maxRight,
                    right
                );


            minUp =
                Mathf.Min(
                    minUp,
                    up
                );


            maxUp =
                Mathf.Max(
                    maxUp,
                    up
                );


            maxDepth =
                Mathf.Max(
                    maxDepth,
                    depth
                );
        }


        /*
         * Slightly move the border toward the camera so it
         * appears in front of the card instead of clipping
         * into the mesh.
         */

        Vector3 borderCenter =
            center +
            cameraForward *
            (maxDepth + borderOffset);


        float pulse =
            Mathf.Sin(
                Time.time *
                borderPulseSpeed
            ) *
            borderPulseAmount;


        float padding =
            borderOffset +
            pulse;


        Vector3 bottomLeft =
            borderCenter +
            cameraRight *
            (minRight - padding) +
            cameraUp *
            (minUp - padding);


        Vector3 bottomRight =
            borderCenter +
            cameraRight *
            (maxRight + padding) +
            cameraUp *
            (minUp - padding);


        Vector3 topRight =
            borderCenter +
            cameraRight *
            (maxRight + padding) +
            cameraUp *
            (maxUp + padding);


        Vector3 topLeft =
            borderCenter +
            cameraRight *
            (minRight - padding) +
            cameraUp *
            (maxUp + padding);


        accessibilityBorder.SetPosition(
            0,
            bottomLeft
        );


        accessibilityBorder.SetPosition(
            1,
            bottomRight
        );


        accessibilityBorder.SetPosition(
            2,
            topRight
        );


        accessibilityBorder.SetPosition(
            3,
            topLeft
        );


        accessibilityBorder.startColor =
            accessibilityBorderColor;


        accessibilityBorder.endColor =
            accessibilityBorderColor;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!accessibilityHighlighted)
            return;


        UpdateAccessibilityBorder();
    }


    // =========================================================
    // SEQUENCE FLASH
    // =========================================================

    public IEnumerator PlaySequenceFlash(
        float duration
    )
    {
        if (cardRenderer == null)
            yield break;


        if (cardMaterial == null)
        {
            cardMaterial =
                cardRenderer.material;
        }


        bool visualEnabled =
            AccessibilityManager.Instance != null &&
            AccessibilityManager.Instance
                .IsVisualAssistanceEnabled();


        /*
         * VISUAL ASSISTANCE
         *
         * Only THIS card gets highlighted.
         */

        if (visualEnabled)
        {
            SetAccessibilityHighlight(true);
        }


        Color normalColor =
            originalColor;


        Color flashColor =
            Color.Lerp(
                normalColor,
                Color.white,
                0.75f
            );


        cardMaterial.color =
            flashColor;


        yield return new WaitForSeconds(
            duration
        );


        cardMaterial.color =
            normalColor;


        if (visualEnabled)
        {
            SetAccessibilityHighlight(false);
        }
    }


    // =========================================================
    // MOUSE CLICK
    // =========================================================

    private void OnMouseDown()
    {
        if (controller == null)
            return;


        controller.CardClicked(
            cardNumber
        );
    }
}