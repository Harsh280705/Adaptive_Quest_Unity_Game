using UnityEngine;
using UnityEngine.SceneManagement;

public class AccessibilityContinue : MonoBehaviour
{
    [SerializeField] private string level1SceneName = "Level1";

    public void ContinueToLevel1()
    {
        AccessibilityManager manager =
            AccessibilityManager.Instance;

        if (manager != null)
        {
            manager.ApplySettings();
        }
        else
        {
            Debug.LogError("AccessibilityManager not found!");
        }

        SceneManager.LoadScene(level1SceneName);
    }
}