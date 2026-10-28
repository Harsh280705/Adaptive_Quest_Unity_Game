using UnityEngine;
using TMPro;

public class EscapeTimerHUD : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TMP_Text timerText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private void Start()
    {
        HideTimer();

        if (timerText != null)
        {
            timerText.color = normalColor;
        }
    }

    public void StartEscapeTimer()
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
        }

        if (timerText != null)
        {
            timerText.color = normalColor;
        }
    }

    public void UpdateTimer(float remainingTime)
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(remainingTime);

        if (seconds < 0)
        {
            seconds = 0;
        }

        timerText.text = seconds.ToString();

        // Make 3, 2 and 1 red
        if (seconds <= 3)
        {
            timerText.color = warningColor;
        }
        else
        {
            timerText.color = normalColor;
        }
    }

    public void StopEscapeTimer()
    {
        HideTimer();

        if (timerText != null)
        {
            timerText.color = normalColor;
        }
    }

    private void HideTimer()
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
    }
}