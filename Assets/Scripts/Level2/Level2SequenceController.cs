using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level2SequenceController : MonoBehaviour
{
    // =========================================================
    // LEVEL 2 PLAYER LOCK
    // =========================================================

    [Header("LEVEL 2 PLAYER LOCK")]
    [SerializeField] private GameObject player;

    private bool level2PlayerLocked;

    private Vector3 lockedPlayerPosition;

    private Quaternion lockedPlayerRotation;

    private Rigidbody playerRigidbody;

    private PlayerController playerController;


    // =========================================================
    // SEQUENCE CARDS
    // =========================================================

    [Header("SEQUENCE CARDS")]
    [SerializeField] private Level2SequenceCard[] sequenceCards;


    // =========================================================
    // SEQUENCE SETTINGS
    // =========================================================

    [Header("SEQUENCE SETTINGS")]
    [SerializeField] private int sequenceLength = 8;

    [SerializeField] private float showTime = 1.5f;

    [SerializeField] private float gapTime = 0.8f;


    // =========================================================
    // ACCESSIBILITY
    // =========================================================

    [Header("ACCESSIBILITY")]
    [SerializeField] private bool allowArrowNavigation = true;


    // =========================================================
    // INDIVIDUAL CARD COLORS
    // =========================================================

    [Header("INDIVIDUAL CARD COLORS")]

    [SerializeField] private Color color1 = Color.red;

    [SerializeField] private Color color2 =
        new Color(1f, 0.5f, 0f);

    [SerializeField] private Color color3 =
        Color.yellow;

    [SerializeField] private Color color4 =
        Color.green;

    [SerializeField] private Color color5 =
        Color.cyan;

    [SerializeField] private Color color6 =
        Color.blue;

    [SerializeField] private Color color7 =
        new Color(0.6f, 0.2f, 1f);

    [SerializeField] private Color color8 =
        Color.magenta;


    // =========================================================
    // SEQUENCE FLASH
    // =========================================================

    [Header("SEQUENCE FLASH")]
    [SerializeField] private Color sequenceFlashColor =
        Color.white;


    // =========================================================
    // OPTIONAL POPUPS
    // =========================================================

    [Header("OPTIONAL POPUPS")]

    [SerializeField] private GameObject successPopup;

    [SerializeField] private GameObject retryPopup;


    // =========================================================
    // SEQUENCE DATA
    // =========================================================

    private readonly List<int> correctSequence =
        new List<int>();

    private readonly List<int> playerInput =
        new List<int>();

    private bool acceptingInput = false;

    private int selectedCardIndex = 0;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        EnableMouseInteraction();

        ValidateCards();

        ApplyCardColors();

        if (successPopup != null)
        {
            successPopup.SetActive(false);
        }

        if (retryPopup != null)
        {
            retryPopup.SetActive(false);
        }

        // IMPORTANT:
        // Lock the player for the ENTIRE Level 2.
        LockPlayer();

        StartCoroutine(StartSequence());
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!acceptingInput)
        {
            return;
        }

        // Keep mouse available for Level 2.
        EnableMouseInteraction();

        bool simplifiedControls = false;

        if (AccessibilityManager.Instance != null)
        {
            simplifiedControls =
                AccessibilityManager.Instance
                    .IsSimplifiedControlsEnabled();
        }

        /*
         * Arrow navigation works when:
         *
         * 1. It is enabled in the Inspector
         *
         * OR
         *
         * 2. Simplified Controls was selected
         *    on the Accessibility screen.
         */

        if (allowArrowNavigation || simplifiedControls)
        {
            HandleArrowNavigation();
        }
    }


    // =========================================================
    // MOUSE / CURSOR
    // =========================================================

    private void EnableMouseInteraction()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // =========================================================
    // CARD SETUP
    // =========================================================

    private void ValidateCards()
    {
        if (sequenceCards == null ||
            sequenceCards.Length != 8)
        {
            Debug.LogError(
                "LEVEL 2 ERROR: Exactly 8 cards must be assigned."
            );

            return;
        }

        for (int i = 0; i < sequenceCards.Length; i++)
        {
            if (sequenceCards[i] == null)
            {
                Debug.LogError(
                    "LEVEL 2 ERROR: Card " +
                    (i + 1) +
                    " is not assigned."
                );
            }
            else
            {
                sequenceCards[i].Setup(
                    this,
                    i + 1
                );
            }
        }
    }


    // =========================================================
    // APPLY CARD COLORS
    // =========================================================

    private void ApplyCardColors()
    {
        if (sequenceCards == null ||
            sequenceCards.Length != 8)
        {
            return;
        }

        Color[] colors =
        {
            color1,
            color2,
            color3,
            color4,
            color5,
            color6,
            color7,
            color8
        };

        for (int i = 0; i < 8; i++)
        {
            if (sequenceCards[i] != null)
            {
                sequenceCards[i].SetCardColor(
                    colors[i]
                );
            }
        }
    }


    // =========================================================
    // PLAYER LOCK
    // =========================================================

    private void LockPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "LEVEL 2: Player is not assigned."
            );

            return;
        }

        playerController =
            player.GetComponent<PlayerController>();

        playerRigidbody =
            player.GetComponent<Rigidbody>();


        // -----------------------------------------------------
        // REMEMBER EXACT PLAYER POSITION
        // -----------------------------------------------------

        lockedPlayerPosition =
            player.transform.position;

        lockedPlayerRotation =
            player.transform.rotation;


        // -----------------------------------------------------
        // ENABLE LOCK
        // -----------------------------------------------------

        level2PlayerLocked = true;


        // -----------------------------------------------------
        // DISABLE PLAYER CONTROLLER
        // -----------------------------------------------------

        if (playerController != null)
        {
            playerController.enabled = false;
        }


        // -----------------------------------------------------
        // STOP EXISTING PHYSICS MOVEMENT
        // -----------------------------------------------------

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity =
                Vector3.zero;

            playerRigidbody.angularVelocity =
                Vector3.zero;

            playerRigidbody.isKinematic = true;
        }


        Debug.Log(
            "LEVEL 2: Player movement LOCKED."
        );
    }


    // =========================================================
    // KEEP PLAYER LOCKED
    // =========================================================

    private void LateUpdate()
    {
        if (!level2PlayerLocked)
        {
            return;
        }

        if (player == null)
        {
            return;
        }


        // -----------------------------------------------------
        // KEEP PLAYER EXACTLY WHERE LEVEL 2 STARTED
        // -----------------------------------------------------

        player.transform.position =
            lockedPlayerPosition;

        player.transform.rotation =
            lockedPlayerRotation;


        // -----------------------------------------------------
        // STOP PHYSICS MOVEMENT
        // -----------------------------------------------------

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity =
                Vector3.zero;

            playerRigidbody.angularVelocity =
                Vector3.zero;
        }
    }


    // =========================================================
    // UNLOCK PLAYER
    // =========================================================
    //
    // IMPORTANT:
    // This method is intentionally NOT called anywhere
    // during Level 2.
    //
    // It exists only so the controller can be safely reused
    // later if needed.
    // =========================================================

    private void UnlockPlayer()
    {
        level2PlayerLocked = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;

            playerRigidbody.linearVelocity =
                Vector3.zero;

            playerRigidbody.angularVelocity =
                Vector3.zero;
        }

        if (playerController != null)
        {
            playerController.enabled = true;

            Debug.Log(
                "LEVEL 2: Player movement enabled."
            );
        }

        EnableMouseInteraction();
    }


    // =========================================================
    // GENERATE RANDOM SEQUENCE
    // =========================================================

    private void GenerateSequence()
    {
        correctSequence.Clear();

        List<int> numbers =
            new List<int>();

        for (int i = 1; i <= 8; i++)
        {
            numbers.Add(i);
        }


        // -----------------------------------------------------
        // SHUFFLE ALL 8 CARDS
        // -----------------------------------------------------

        for (int i = 0; i < numbers.Count; i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    numbers.Count
                );

            int temp =
                numbers[i];

            numbers[i] =
                numbers[randomIndex];

            numbers[randomIndex] =
                temp;
        }


        // -----------------------------------------------------
        // CREATE SEQUENCE
        // -----------------------------------------------------

        int amount =
            Mathf.Clamp(
                sequenceLength,
                1,
                8
            );

        for (int i = 0; i < amount; i++)
        {
            correctSequence.Add(
                numbers[i]
            );
        }


        Debug.Log(
            "LEVEL 2 SEQUENCE: " +
            string.Join(
                " → ",
                correctSequence
            )
        );
    }


    // =========================================================
    // SHOW SEQUENCE
    // =========================================================

    private IEnumerator StartSequence()
    {
        acceptingInput = false;

        playerInput.Clear();

        GenerateSequence();

        Debug.Log(
            "LEVEL 2: MEMORIZE THE SEQUENCE"
        );

        yield return new WaitForSeconds(1f);


        for (
            int i = 0;
            i < correctSequence.Count;
            i++
        )
        {
            int cardNumber =
                correctSequence[i];

            Level2SequenceCard card =
                GetCard(cardNumber);

            if (card != null)
            {
                Debug.Log(
                    "LEVEL 2: Showing Card " +
                    cardNumber
                );

                yield return StartCoroutine(
                    card.PlaySequenceFlash(
                        showTime
                    )
                );
            }

            yield return new WaitForSeconds(
                gapTime
            );
        }


        Debug.Log(
            "LEVEL 2: YOUR TURN"
        );

        BeginPlayerInput();
    }


    // =========================================================
    // PLAYER INPUT
    // =========================================================

    private void BeginPlayerInput()
    {
        playerInput.Clear();

        acceptingInput = true;

        selectedCardIndex = 0;

        EnableMouseInteraction();

        HighlightSelectedCard();

        Debug.Log(
            "LEVEL 2: Player can now enter the sequence."
        );
    }


    // =========================================================
    // CARD CLICKED
    // =========================================================

    public void CardClicked(int cardNumber)
    {
        if (!acceptingInput)
        {
            return;
        }

        if (cardNumber < 1 ||
            cardNumber > 8)
        {
            return;
        }

        Debug.Log(
            "LEVEL 2: Player selected Card " +
            cardNumber
        );

        playerInput.Add(
            cardNumber
        );

        CheckCurrentInput();
    }


    // =========================================================
    // CHECK CURRENT INPUT
    // =========================================================

    private void CheckCurrentInput()
    {
        int currentIndex =
            playerInput.Count - 1;

        if (
            currentIndex < 0 ||
            currentIndex >= correctSequence.Count
        )
        {
            return;
        }


        // =====================================================
        // WRONG
        // =====================================================

        if (
            playerInput[currentIndex] !=
            correctSequence[currentIndex]
        )
        {
            StartCoroutine(
                HandleWrongAnswer()
            );

            return;
        }


        // =====================================================
        // COMPLETED CORRECTLY
        // =====================================================

        if (
            playerInput.Count ==
            correctSequence.Count
        )
        {
            StartCoroutine(
                HandleCorrectAnswer()
            );
        }
    }


    // =========================================================
    // CORRECT ANSWER
    // =========================================================

    private IEnumerator HandleCorrectAnswer()
    {
        acceptingInput = false;

        Debug.Log(
            "LEVEL 2: CORRECT SEQUENCE!"
        );

        AccessibilityFeedback.PlaySuccessFeedback();

        yield return new WaitForSeconds(0.5f);

        if (successPopup != null)
        {
            successPopup.SetActive(true);
        }
        else
        {
            Debug.Log(
                "LEVEL 2 SUCCESS: " +
                "Thank you for playing the game!"
            );
        }

        EnableMouseInteraction();

        // IMPORTANT:
        //
        // DO NOT call UnlockPlayer().
        //
        // Player movement remains disabled for the
        // entire Level 2.
    }


    // =========================================================
    // WRONG ANSWER
    // =========================================================

    private IEnumerator HandleWrongAnswer()
    {
        acceptingInput = false;

        AccessibilityFeedback.PlayErrorFeedback();

        Debug.Log(
            "LEVEL 2: WRONG SEQUENCE - TRY AGAIN"
        );

        if (retryPopup != null)
        {
            retryPopup.SetActive(true);

            EnableMouseInteraction();

            yield return new WaitForSeconds(
                1.5f
            );

            retryPopup.SetActive(false);
        }
        else
        {
            Debug.Log(
                "LEVEL 2: TRY AGAIN"
            );

            yield return new WaitForSeconds(
                1.5f
            );
        }


        // Demonstrate a new sequence again.
        StartCoroutine(
            StartSequence()
        );
    }


    // =========================================================
    // GET CARD
    // =========================================================

    private Level2SequenceCard GetCard(
        int number
    )
    {
        if (
            number < 1 ||
            number > 8
        )
        {
            return null;
        }

        if (
            sequenceCards == null ||
            sequenceCards.Length != 8
        )
        {
            return null;
        }

        return sequenceCards[
            number - 1
        ];
    }


    // =========================================================
    // ACCESSIBILITY - ARROW NAVIGATION
    // =========================================================

    private void HandleArrowNavigation()
    {
        if (Keyboard.current == null)
        {
            return;
        }


        // -----------------------------------------------------
        // RIGHT
        // -----------------------------------------------------

        if (
            Keyboard.current.rightArrowKey
                .wasPressedThisFrame
        )
        {
            selectedCardIndex++;

            if (selectedCardIndex > 7)
            {
                selectedCardIndex = 0;
            }

            HighlightSelectedCard();
        }


        // -----------------------------------------------------
        // LEFT
        // -----------------------------------------------------

        if (
            Keyboard.current.leftArrowKey
                .wasPressedThisFrame
        )
        {
            selectedCardIndex--;

            if (selectedCardIndex < 0)
            {
                selectedCardIndex = 7;
            }

            HighlightSelectedCard();
        }


        // -----------------------------------------------------
        // DOWN
        // -----------------------------------------------------

        if (
            Keyboard.current.downArrowKey
                .wasPressedThisFrame
        )
        {
            MoveVertical(1);

            HighlightSelectedCard();
        }


        // -----------------------------------------------------
        // UP
        // -----------------------------------------------------

        if (
            Keyboard.current.upArrowKey
                .wasPressedThisFrame
        )
        {
            MoveVertical(-1);

            HighlightSelectedCard();
        }


        // -----------------------------------------------------
        // ENTER
        // -----------------------------------------------------

        if (
            Keyboard.current.enterKey
                .wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey
                .wasPressedThisFrame
        )
        {
            SelectHighlightedCard();
        }
    }


    // =========================================================
    // VERTICAL NAVIGATION
    // =========================================================

    private void MoveVertical(int direction)
    {
        /*
         * Layout:
         *
         * 1   2   3   4
         *
         * 5   6   7   8
         */

        int row =
            selectedCardIndex / 4;

        int column =
            selectedCardIndex % 4;

        row += direction;

        if (row < 0)
        {
            row = 1;
        }

        if (row > 1)
        {
            row = 0;
        }

        selectedCardIndex =
            row * 4 + column;
    }


    // =========================================================
    // ACCESSIBILITY HIGHLIGHT
    // =========================================================

    private void HighlightSelectedCard()
    {
        if (sequenceCards == null)
        {
            return;
        }

        for (
            int i = 0;
            i < sequenceCards.Length;
            i++
        )
        {
            if (sequenceCards[i] != null)
            {
                sequenceCards[i]
                    .SetAccessibilityHighlight(
                        i == selectedCardIndex
                    );
            }
        }
    }


    // =========================================================
    // SELECT HIGHLIGHTED CARD
    // =========================================================

    private void SelectHighlightedCard()
    {
        if (
            selectedCardIndex < 0 ||
            selectedCardIndex >= 8
        )
        {
            return;
        }

        int cardNumber =
            selectedCardIndex + 1;

        CardClicked(
            cardNumber
        );
    }
}