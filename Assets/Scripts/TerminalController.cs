using UnityEngine;

public class TerminalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupSystem pickupSystem;
    [SerializeField] private GameObject heldCrystal;
    [SerializeField] private GameObject terminalCrystal;
    [SerializeField] private Light terminalGlow;

    [Header("Exit")]
    [SerializeField] private ExitTimer exitTimer;
    [SerializeField] private EscapeTimerHUD escapeTimerHUD;

    [Header("Audio")]
    [SerializeField] private AudioSource insertAudio;

    private bool activated = false;

    public void ActivateTerminal()
    {
        // Prevent activating the terminal more than once
        if (activated)
            return;

        // Make sure the player actually has the crystal
        if (pickupSystem == null || !pickupSystem.HasCrystal)
            return;

        activated = true;

        // --------------------------------------------------
        // 1. Remove crystal from player's hand
        // --------------------------------------------------
        if (heldCrystal != null)
        {
            heldCrystal.SetActive(false);
        }

        // --------------------------------------------------
        // 2. Show crystal inside the terminal
        // --------------------------------------------------
        if (terminalCrystal != null)
        {
            terminalCrystal.SetActive(true);
        }

        // --------------------------------------------------
        // 3. Turn on terminal glow
        // --------------------------------------------------
        if (terminalGlow != null)
        {
            terminalGlow.enabled = true;
        }

        // --------------------------------------------------
        // 4. Play crystal insertion / accepted sound
        // --------------------------------------------------
        if (insertAudio != null)
        {
            insertAudio.Play();
        }

        // --------------------------------------------------
        // 5. Start the 10-second escape timer
        // --------------------------------------------------
        if (exitTimer != null)
        {
            exitTimer.StartExitTimer();
        }

        Debug.Log("Crystal inserted. EXIT TIMER STARTED!");
    }
}