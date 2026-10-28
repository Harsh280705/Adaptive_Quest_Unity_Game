using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ExitTimer exitTimer;

    [Header("Scene")]
    [SerializeField] private string nextScene = "Level02_EmergencyEscape";

    private bool hasExited = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the Player
        if (!other.CompareTag("Player"))
            return;

        // Prevent multiple triggers
        if (hasExited)
            return;

        // Make sure the exit timer is active
        if (exitTimer == null)
            return;

        hasExited = true;

        // Complete Level 1
        exitTimer.CompleteLevel();

        // Load Level 2
        SceneManager.LoadScene(nextScene);
    }
}