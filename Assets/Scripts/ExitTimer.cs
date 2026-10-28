using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float timeLimit = 10f;

    [Header("UI")]
    [SerializeField] private EscapeTimerHUD escapeTimerHUD;

    [Header("Exit")]
    [SerializeField] private DoorController exitDoor;

    private float timeRemaining;
    private bool timerActive = false;
    private bool levelCompleted = false;

    public void StartExitTimer()
    {
        if (timerActive || levelCompleted)
            return;

        timerActive = true;
        levelCompleted = false;
        timeRemaining = timeLimit;

        // Open the exit door
        if (exitDoor != null)
        {
            exitDoor.ToggleDoor();
        }

        // Show the timer
        if (escapeTimerHUD != null)
        {
            escapeTimerHUD.StartEscapeTimer();
            escapeTimerHUD.UpdateTimer(timeRemaining);
        }

        Debug.Log("EXIT OPEN! You have " + timeLimit + " seconds!");
    }

    private void Update()
    {
        if (!timerActive || levelCompleted)
            return;

        timeRemaining -= Time.deltaTime;

        // Update timer UI
        if (escapeTimerHUD != null)
        {
            escapeTimerHUD.UpdateTimer(timeRemaining);
        }

        // Time expired
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerActive = false;

            if (escapeTimerHUD != null)
            {
                escapeTimerHUD.UpdateTimer(0f);
            }

            Debug.Log("Time expired! Restarting Level 01.");

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    public void CompleteLevel()
    {
        if (!timerActive)
            return;

        levelCompleted = true;
        timerActive = false;

        if (escapeTimerHUD != null)
        {
            escapeTimerHUD.StopEscapeTimer();
        }

        Debug.Log("Level 01 completed!");
    }
}