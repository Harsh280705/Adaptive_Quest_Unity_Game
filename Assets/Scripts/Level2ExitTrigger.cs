using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2ExitTrigger : MonoBehaviour
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

        Debug.Log("PLAYER REACHED EXIT - LOADING LEVEL 2");

        SceneManager.LoadScene(nextScene);
    }
}