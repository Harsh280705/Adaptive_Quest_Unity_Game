using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Pickup")]
    [SerializeField] private PickupSystem pickupSystem;

    [Header("Terminal UI")]
    [SerializeField] private TerminalDisplayController terminalDisplay;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        // -----------------------------------------------------
        // FIND PLAYER CAMERA
        // -----------------------------------------------------

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }


        // -----------------------------------------------------
        // FIND PICKUP SYSTEM
        // -----------------------------------------------------

        if (pickupSystem == null)
        {
            pickupSystem = GetComponent<PickupSystem>();
        }
    }


    private void Update()
    {
        // -----------------------------------------------------
        // MAKE SURE CAMERA EXISTS
        // -----------------------------------------------------

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
            return;


        // -----------------------------------------------------
        // INTERACTION INPUT
        // -----------------------------------------------------

        bool interactPressed = false;


        // =====================================================
        // NORMAL INTERACTION
        // =====================================================

        // E always works.
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactPressed = true;
        }


        // =====================================================
        // SIMPLIFIED CONTROLS
        // =====================================================

        // When Simplified Controls is enabled,
        // SPACE becomes an additional interaction key.
        //
        // E continues to work normally.
        //
        // This section is intentionally unchanged.
        if (AccessibilityManager.Instance != null &&
            AccessibilityManager.Instance.IsSimplifiedControlsEnabled())
        {
            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                interactPressed = true;
            }
        }


        // =====================================================
        // EXECUTE INTERACTION
        // =====================================================

        if (interactPressed)
        {
            TryInteract();
        }
    }


    // =========================================================
    // MAIN INTERACTION
    // =========================================================

    private void TryInteract()
    {
        // -----------------------------------------------------
        // CREATE FORWARD RAY
        // -----------------------------------------------------

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );


        // -----------------------------------------------------
        // PERFORM REAL RAYCAST
        // -----------------------------------------------------

        RaycastHit hit;

        bool objectDetected = Physics.Raycast(
            ray,
            out hit,
            interactionDistance
        );


        // -----------------------------------------------------
        // NOTHING DETECTED
        // -----------------------------------------------------

        if (!objectDetected)
        {
            Debug.Log("Nothing to interact with.");
            return;
        }


        // =====================================================
        // 1. ACCESS CRYSTAL
        // =====================================================

        if (hit.collider.CompareTag("AccessCrystal"))
        {
            if (pickupSystem != null)
            {
                pickupSystem.CollectCrystal();

                PlaySuccessFeedback();

                if (AccessibilityRuntime.InstanceExists)
                {
                    AccessibilityRuntime
                        .GetInstance()
                        .NotifyCrystalCollected();
                }

                Debug.Log("Access Crystal collected.");

                return;
            }
        }


        // =====================================================
        // 2. CRYSTAL TERMINAL
        // =====================================================

        TerminalController terminal =
            hit.collider.GetComponentInParent<TerminalController>();


        if (terminal != null)
        {
            // -------------------------------------------------
            // PLAYER DOES NOT HAVE CRYSTAL
            // -------------------------------------------------

            if (pickupSystem == null ||
                !pickupSystem.HasCrystal)
            {
                Debug.Log(
                    "You need the Access Crystal first!"
                );

                if (terminalDisplay != null)
                {
                    terminalDisplay.ShowInstruction();
                }

                return;
            }


            // -------------------------------------------------
            // PLAYER HAS CRYSTAL
            // -------------------------------------------------

            terminal.ActivateTerminal();

            if (terminalDisplay != null)
            {
                terminalDisplay.ShowAccepted();
            }

            PlaySuccessFeedback();

            if (AccessibilityRuntime.InstanceExists)
            {
                AccessibilityRuntime
                    .GetInstance()
                    .NotifyTerminalActivated();
            }

            Debug.Log(
                "Crystal Terminal activated."
            );

            return;
        }


        // =====================================================
        // 3. DOOR
        // =====================================================

        DoorController door =
            hit.collider.GetComponentInParent<DoorController>();


        if (door != null)
        {
            // -------------------------------------------------
            // CHECK CRYSTAL REQUIREMENT
            // -------------------------------------------------

            if (pickupSystem != null &&
                !pickupSystem.HasCrystal)
            {
                Debug.Log(
                    "You need the Access Crystal first!"
                );

                return;
            }


            // -------------------------------------------------
            // OPEN / CLOSE DOOR
            // -------------------------------------------------

            door.ToggleDoor();

            PlaySuccessFeedback();

            if (AccessibilityRuntime.InstanceExists)
            {
                AccessibilityRuntime
                    .GetInstance()
                    .NotifyDoorInteracted();
            }

            Debug.Log(
                "Door interaction completed."
            );

            return;
        }


        // =====================================================
        // NOTHING USABLE FOUND
        // =====================================================

        Debug.Log(
            "The detected object is not interactable."
        );
    }


    // =========================================================
    // SUCCESS FEEDBACK
    // =========================================================

    private void PlaySuccessFeedback()
    {
        // Small interaction feedback.
        //
        // This method intentionally does not change
        // Simplified Controls or Visual Assistance.
        //
        // AccessibilityRuntime handles the actual
        // accessibility notifications separately.

        Debug.Log("Interaction successful.");
    }
}