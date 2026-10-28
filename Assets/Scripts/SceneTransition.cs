using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string nextScene = "Level02_EmergencyEscape";

    private bool hasTransitioned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTransitioned)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTransitioned = true;
        SceneManager.LoadScene(nextScene);
    }
}