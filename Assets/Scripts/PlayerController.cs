    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;

        [Header("Simplified Controls")]
        [SerializeField] private float simplifiedMoveSpeed = 4f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private Vector3 velocity;

        private void Awake()
        {
            characterController =
                GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleMovement();
            HandleGravity();
        }

        private void HandleMovement()
        {
            Vector2 input = Vector2.zero;

            if (Keyboard.current != null)
            {
                // =================================================
                // WASD CONTROLS
                // =================================================

                float horizontal =
                    (Keyboard.current.dKey.isPressed ? 1f : 0f)
                    -
                    (Keyboard.current.aKey.isPressed ? 1f : 0f);

                float vertical =
                    (Keyboard.current.wKey.isPressed ? 1f : 0f)
                    -
                    (Keyboard.current.sKey.isPressed ? 1f : 0f);

                input.x = horizontal;
                input.y = vertical;

                // =================================================
                // SIMPLIFIED CONTROLS
                // =================================================
                // When enabled, ARROW KEYS also work.
                //
                // This gives the player another simple control
                // method without changing the existing controls.
                // =================================================

                if (AccessibilityManager.Instance != null &&
                    AccessibilityManager.Instance.IsSimplifiedControlsEnabled())
                {
                    float arrowHorizontal =
                        (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
                        -
                        (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);

                    float arrowVertical =
                        (Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                        -
                        (Keyboard.current.downArrowKey.isPressed ? 1f : 0f);

                    // Use arrow-key input if it is being used.
                    if (Mathf.Abs(arrowHorizontal) > 0f ||
                        Mathf.Abs(arrowVertical) > 0f)
                    {
                        input.x = arrowHorizontal;
                        input.y = arrowVertical;
                    }
                }
            }

            // =====================================================
            // PREVENT DIAGONAL MOVEMENT FROM BEING FASTER
            // =====================================================

            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            // =====================================================
            // WORLD MOVEMENT
            // =====================================================

            Vector3 move =
                transform.right * input.x +
                transform.forward * input.y;

            // =====================================================
            // MOVEMENT SPEED
            // =====================================================

            float currentMoveSpeed = moveSpeed;

            if (AccessibilityManager.Instance != null &&
                AccessibilityManager.Instance.IsSimplifiedControlsEnabled())
            {
                currentMoveSpeed = simplifiedMoveSpeed;
            }

            characterController.Move(
                move *
                currentMoveSpeed *
                Time.deltaTime
            );
        }

        private void HandleGravity()
        {
            if (characterController.isGrounded &&
                velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;

            characterController.Move(
                velocity *
                Time.deltaTime
            );
        }
    }