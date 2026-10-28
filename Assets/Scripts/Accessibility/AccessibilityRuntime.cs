using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccessibilityRuntime : MonoBehaviour
{
    private static AccessibilityRuntime instance;


    // =========================================================
    // UI
    // =========================================================

    private Canvas accessibilityCanvas;

    private GameObject arrowObject;
    private Text arrowText;

    private GameObject instructionObject;
    private Text instructionText;


    // =========================================================
    // PLAYER / CAMERA
    // =========================================================

    private Camera currentCamera;
    private PlayerController currentPlayer;


    // =========================================================
    // VISUAL ASSISTANCE
    // =========================================================

    [Header("Visual Assistance")]

    [SerializeField]
    private float targetSearchDistance = 35f;

    [SerializeField]
    private float visualUpdateInterval = 0.12f;


    private float visualUpdateTimer;


    // =========================================================
    // ARROW
    // =========================================================

    [Header("Arrow")]

    [SerializeField]
    private float arrowDistanceFromCenter = 170f;

    [SerializeField]
    private float arrowSize = 80f;


    // =========================================================
    // CURRENT TARGET
    // =========================================================

    private Transform currentTarget;

    private AccessibilityVisualAssist currentHighlight;

    private AccessibilityRedBorder currentRedBorder;

    private bool currentRedBorderWasAdded;

    private bool currentHighlightWasAdded;

    private string currentTargetMessage = "";


    // =========================================================
    // LEVEL 1 PROGRESSION
    // =========================================================
    //
    // 0 = Access Crystal
    // 1 = Crystal Terminal
    // 2 = Door
    // 3 = Finished
    //

    private int level1Stage = 0;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (
            instance != null &&
            instance != this
        )
        {
            Destroy(gameObject);
            return;
        }


        instance = this;

        DontDestroyOnLoad(
            gameObject
        );


        SceneManager.sceneLoaded +=
            OnSceneLoaded;


        AccessibilityManager
            .AccessibilitySettingsChanged +=
            ApplyAccessibility;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ApplyAccessibility();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (
            AccessibilityManager.Instance ==
            null
        )
        {
            return;
        }


        if (
            !AccessibilityManager.Instance
                .IsVisualAssistanceEnabled()
        )
        {
            return;
        }


        visualUpdateTimer -=
            Time.unscaledDeltaTime;


        if (visualUpdateTimer <= 0f)
        {
            visualUpdateTimer =
                visualUpdateInterval;


            RefreshPlayerAndCamera();


            /*
             * Level 2 has its own visual assistance.
             *
             * The runtime arrow/Level 1 guide should
             * not interfere with the sequence cards.
             */

            if (
                FindAnyObjectByType<
                    Level2SequenceController
                >() != null
            )
            {
                ClearWorldTarget();

                HideVisualGuide();

                return;
            }


            UpdateVisualAssistance();
        }


        UpdateArrow();
    }


    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;

            AccessibilityManager
                .AccessibilitySettingsChanged -=
                ApplyAccessibility;
        }


        ClearWorldTarget();
    }


    // =========================================================
    // SCENE LOADED
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        currentPlayer = null;

        currentCamera = null;

        level1Stage = 0;


        ClearWorldTarget();

        HideVisualGuide();


        Invoke(
            nameof(ApplyAccessibility),
            0.2f
        );
    }


    // =========================================================
    // APPLY ACCESSIBILITY
    // =========================================================

    public void ApplyAccessibility()
    {
        if (
            AccessibilityManager.Instance ==
            null
        )
        {
            return;
        }


        RefreshPlayerAndCamera();


        bool visualEnabled =
            AccessibilityManager.Instance
                .IsVisualAssistanceEnabled();


        if (visualEnabled)
        {
            CreateVisualAssistance();
        }
        else
        {
            ClearWorldTarget();

            RemoveVisualAssistance();
        }
    }


    // =========================================================
    // FIND PLAYER / CAMERA
    // =========================================================

    private void RefreshPlayerAndCamera()
    {
        currentCamera =
            Camera.main;


        if (currentCamera == null)
        {
            Camera[] cameras =
                Camera.allCameras;

            if (
                cameras != null &&
                cameras.Length > 0
            )
            {
                currentCamera =
                    cameras[0];
            }
        }


        if (currentPlayer == null)
        {
            currentPlayer =
                FindAnyObjectByType<
                    PlayerController
                >();
        }
    }


    // =========================================================
    // CREATE UI
    // =========================================================

    private void CreateVisualAssistance()
    {
        if (
            accessibilityCanvas !=
            null
        )
        {
            return;
        }


        GameObject canvasObject =
            new GameObject(
                "AccessibilityVisualCanvas"
            );


        accessibilityCanvas =
            canvasObject.AddComponent<
                Canvas
            >();


        accessibilityCanvas.renderMode =
            RenderMode.ScreenSpaceOverlay;


        accessibilityCanvas.sortingOrder =
            100;


        CanvasScaler scaler =
            canvasObject.AddComponent<
                CanvasScaler
            >();


        scaler.uiScaleMode =
            CanvasScaler.ScaleMode
                .ScaleWithScreenSize;


        scaler.referenceResolution =
            new Vector2(
                1920f,
                1080f
            );


        canvasObject.AddComponent<
            GraphicRaycaster
        >();


        // -----------------------------------------------------
        // ARROW
        // -----------------------------------------------------

        arrowObject =
            new GameObject(
                "AccessibilityDirectionArrow"
            );


        arrowObject.transform.SetParent(
            accessibilityCanvas.transform,
            false
        );


        arrowText =
            arrowObject.AddComponent<
                Text
            >();


        arrowText.text =
            "➤";


        arrowText.font =
            Resources.GetBuiltinResource<
                Font
            >(
                "LegacyRuntime.ttf"
            );


        arrowText.fontSize =
            Mathf.RoundToInt(
                arrowSize
            );


        arrowText.alignment =
            TextAnchor.MiddleCenter;


        arrowText.horizontalOverflow =
            HorizontalWrapMode.Overflow;


        arrowText.verticalOverflow =
            VerticalWrapMode.Overflow;


        arrowText.color =
            new Color(
                1f,
                0.85f,
                0.05f,
                0.95f
            );


        arrowText.raycastTarget =
            false;


        RectTransform arrowRect =
            arrowObject.GetComponent<
                RectTransform
            >();


        arrowRect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );


        arrowRect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );


        arrowRect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );


        arrowRect.sizeDelta =
            new Vector2(
                arrowSize * 1.5f,
                arrowSize * 1.5f
            );


        arrowRect.anchoredPosition =
            Vector2.zero;


        // -----------------------------------------------------
        // INSTRUCTION
        // -----------------------------------------------------

        instructionObject =
            new GameObject(
                "AccessibilityInstruction"
            );


        instructionObject.transform.SetParent(
            accessibilityCanvas.transform,
            false
        );


        instructionText =
            instructionObject.AddComponent<
                Text
            >();


        instructionText.font =
            Resources.GetBuiltinResource<
                Font
            >(
                "LegacyRuntime.ttf"
            );


        instructionText.fontSize =
            30;


        instructionText.alignment =
            TextAnchor.MiddleCenter;


        instructionText.horizontalOverflow =
            HorizontalWrapMode.Wrap;


        instructionText.verticalOverflow =
            VerticalWrapMode.Overflow;


        instructionText.color =
            Color.white;


        instructionText.raycastTarget =
            false;


        RectTransform instructionRect =
            instructionObject.GetComponent<
                RectTransform
            >();


        instructionRect.anchorMin =
            new Vector2(
                0.5f,
                0f
            );


        instructionRect.anchorMax =
            new Vector2(
                0.5f,
                0f
            );


        instructionRect.pivot =
            new Vector2(
                0.5f,
                0f
            );


        instructionRect.anchoredPosition =
            new Vector2(
                0f,
                80f
            );


        instructionRect.sizeDelta =
            new Vector2(
                700f,
                80f
            );


        arrowObject.SetActive(false);

        instructionObject.SetActive(false);
    }


    // =========================================================
    // UPDATE VISUAL ASSISTANCE
    // =========================================================

    private void UpdateVisualAssistance()
    {
        if (currentPlayer == null)
        {
            RefreshPlayerAndCamera();
        }


        if (currentPlayer == null)
            return;


        if (accessibilityCanvas == null)
        {
            CreateVisualAssistance();
        }


        PickupSystem pickup =
            currentPlayer.GetComponent<
                PickupSystem
            >();


        /*
         * If the crystal was collected but the
         * notification hasn't arrived yet,
         * automatically move to terminal stage.
         */

        if (
            level1Stage == 0 &&
            pickup != null &&
            pickup.HasCrystal
        )
        {
            level1Stage = 1;
        }


        FindBestAccessibilityTarget();
    }


    // =========================================================
    // FIND TARGET
    // =========================================================

    private void FindBestAccessibilityTarget()
    {
        Vector3 playerPosition =
            currentPlayer.transform.position;


        Collider[] nearbyColliders =
            Physics.OverlapSphere(
                playerPosition,
                targetSearchDistance
            );


        Transform bestTarget = null;

        string bestMessage = "";

        float bestScore =
            float.MaxValue;


        foreach (
            Collider collider
            in nearbyColliders
        )
        {
            if (collider == null)
                continue;


            // =================================================
            // STAGE 0 - ACCESS CRYSTAL
            // =================================================

            if (
                level1Stage == 0 &&
                collider.CompareTag(
                    "AccessCrystal"
                )
            )
            {
                float distance =
                    Vector3.Distance(
                        playerPosition,
                        collider.transform.position
                    );


                if (
                    distance <
                    bestScore
                )
                {
                    bestScore =
                        distance;

                    bestTarget =
                        GetRendererTransform(
                            collider.transform
                        );

                    bestMessage =
                        "Find the Access Crystal";
                }


                continue;
            }


            // =================================================
            // STAGE 1 - TERMINAL
            // =================================================

            if (level1Stage == 1)
            {
                TerminalController terminal =
                    collider.GetComponentInParent<
                        TerminalController
                    >();


                if (terminal != null)
                {
                    float distance =
                        Vector3.Distance(
                            playerPosition,
                            terminal.transform.position
                        );


                    if (
                        distance <
                        bestScore
                    )
                    {
                        bestScore =
                            distance;

                        bestTarget =
                            terminal.transform;

                        bestMessage =
                            "Go to the Crystal Terminal and press E";
                    }


                    continue;
                }
            }


            // =================================================
            // STAGE 2 - DOOR
            // =================================================

            if (level1Stage == 2)
            {
                DoorController door =
                    collider.GetComponentInParent<
                        DoorController
                    >();


                if (door != null)
                {
                    float distance =
                        Vector3.Distance(
                            playerPosition,
                            door.transform.position
                        );


                    if (
                        distance <
                        bestScore
                    )
                    {
                        bestScore =
                            distance;

                        bestTarget =
                            door.transform;

                        bestMessage =
                            "Go to the door and press E";
                    }
                }
            }
        }


        SetCurrentTarget(
            bestTarget,
            bestMessage
        );
    }


    // =========================================================
    // FIND RENDERER
    // =========================================================

    private Transform GetRendererTransform(
        Transform source
    )
    {
        Renderer renderer =
            source.GetComponentInParent<
                Renderer
            >();


        if (renderer != null)
            return renderer.transform;


        return source;
    }


    // =========================================================
    // SET TARGET
    // =========================================================

    private void SetCurrentTarget(
        Transform target,
        string message
    )
    {
        if (
            currentTarget == target &&
            currentTargetMessage ==
            message
        )
        {
            return;
        }


        ClearWorldTarget();


        currentTarget =
            target;

        currentTargetMessage =
            message;


        if (currentTarget != null)
        {
            EnableTargetHighlight(
                currentTarget
            );

            ShowVisualGuide();
        }
        else
        {
            HideVisualGuide();
        }
    }


    // =========================================================
    // ENABLE TARGET HIGHLIGHT
    // =========================================================

    private void EnableTargetHighlight(
        Transform target
    )
    {
        if (target == null)
            return;


        // =====================================================
        // EXISTING VISUAL ASSIST
        // =====================================================

        currentHighlight =
            target.GetComponent<
                AccessibilityVisualAssist
            >();


        if (currentHighlight == null)
        {
            currentHighlight =
                target.gameObject.AddComponent<
                    AccessibilityVisualAssist
                >();

            currentHighlightWasAdded =
                true;
        }
        else
        {
            currentHighlightWasAdded =
                false;
        }


        currentHighlight
            .SetRuntimeHighlight(true);


        // =====================================================
        // STRONG RED BORDER
        // =====================================================

        currentRedBorder =
            target.GetComponent<
                AccessibilityRedBorder
            >();


        if (currentRedBorder == null)
        {
            currentRedBorder =
                target.gameObject.AddComponent<
                    AccessibilityRedBorder
                >();

            currentRedBorderWasAdded =
                true;
        }
        else
        {
            currentRedBorderWasAdded =
                false;
        }


        currentRedBorder
            .SetRuntimeHighlight(true);
    }


    // =========================================================
    // CLEAR TARGET
    // =========================================================

    private void ClearWorldTarget()
    {
        // =====================================================
        // CLEAR EXISTING VISUAL ASSIST
        // =====================================================

        if (currentHighlight != null)
        {
            if (currentHighlightWasAdded)
            {
                Destroy(
                    currentHighlight
                );
            }
        else
        {
            currentHighlight
                .SetRuntimeHighlight(false);
        }
    }


    // =====================================================
    // CLEAR RED BORDER
    // =====================================================

    if (currentRedBorder != null)
    {
        if (currentRedBorderWasAdded)
        {
            Destroy(
                currentRedBorder
            );
        }
        else
        {
            currentRedBorder
                .SetRuntimeHighlight(false);
        }
    }


    currentHighlight = null;

    currentHighlightWasAdded =
        false;


    currentRedBorder = null;

    currentRedBorderWasAdded =
        false;


    currentTarget = null;

    currentTargetMessage = "";
}


    // =========================================================
    // PROGRESSION NOTIFICATIONS
    // =========================================================

    public void NotifyCrystalCollected()
    {
        level1Stage = 1;

        ClearWorldTarget();
    }


    public void NotifyTerminalActivated()
    {
        level1Stage = 2;

        ClearWorldTarget();
    }


    public void NotifyDoorInteracted()
    {
        level1Stage = 3;

        ClearWorldTarget();

        HideVisualGuide();
    }


    // =========================================================
    // ARROW
    // =========================================================

    private void UpdateArrow()
    {
        if (
            arrowObject == null ||
            currentTarget == null ||
            currentCamera == null
        )
        {
            return;
        }


        Vector3 targetScreenPosition =
            currentCamera.WorldToScreenPoint(
                currentTarget.position
            );


        if (
            targetScreenPosition.z <=
            0f
        )
        {
            PointArrowToDirection(
                currentTarget.position
            );

            return;
        }


        Vector2 screenCenter =
            new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f
            );


        Vector2 targetPosition =
            new Vector2(
                targetScreenPosition.x,
                targetScreenPosition.y
            );


        Vector2 direction =
            targetPosition -
            screenCenter;


        if (
            direction.sqrMagnitude <
            0.01f
        )
        {
            direction =
                Vector2.up;
        }


        direction.Normalize();


        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;


        RectTransform arrowRect =
            arrowObject.GetComponent<
                RectTransform
            >();


        arrowRect.anchoredPosition =
            direction *
            arrowDistanceFromCenter;


        arrowRect.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }


    // =========================================================
    // TARGET BEHIND PLAYER
    // =========================================================

    private void PointArrowToDirection(
        Vector3 worldPosition
    )
    {
        if (currentCamera == null)
            return;


        Vector3 localDirection =
            currentCamera.transform
                .InverseTransformPoint(
                    worldPosition
                );


        Vector2 direction =
            new Vector2(
                localDirection.x,
                localDirection.y
            );


        if (
            direction.sqrMagnitude <
            0.01f
        )
        {
            direction =
                Vector2.up;
        }


        direction.Normalize();


        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;


        RectTransform arrowRect =
            arrowObject.GetComponent<
                RectTransform
            >();


        arrowRect.anchoredPosition =
            direction *
            arrowDistanceFromCenter;


        arrowRect.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }


    // =========================================================
    // SHOW GUIDE
    // =========================================================

    private void ShowVisualGuide()
    {
        if (arrowObject != null)
            arrowObject.SetActive(true);


        if (instructionObject != null)
            instructionObject.SetActive(true);


        if (instructionText != null)
        {
            instructionText.text =
                currentTargetMessage;
        }
    }


    // =========================================================
    // HIDE GUIDE
    // =========================================================

    private void HideVisualGuide()
    {
        if (arrowObject != null)
            arrowObject.SetActive(false);


        if (instructionObject != null)
            instructionObject.SetActive(false);
    }


    // =========================================================
    // REMOVE UI
    // =========================================================

    private void RemoveVisualAssistance()
    {
        if (
            accessibilityCanvas !=
            null
        )
        {
            Destroy(
                accessibilityCanvas.gameObject
            );
        }


        accessibilityCanvas = null;

        arrowObject = null;
        arrowText = null;

        instructionObject = null;
        instructionText = null;
    }


    // =========================================================
    // AUDIO COMPATIBILITY
    // =========================================================

    public void PlayAccessibilityTone(
        bool success
    )
    {
        if (success)
        {
            AccessibilityFeedback
                .PlaySuccessFeedback();
        }
        else
        {
            AccessibilityFeedback
                .PlayErrorFeedback();
        }
    }


    // =========================================================
    // PUBLIC INSTANCE
    // =========================================================

    public static bool InstanceExists
    {
        get
        {
            return instance != null;
        }
    }


    public static AccessibilityRuntime
        GetInstance()
    {
        return instance;
    }
}