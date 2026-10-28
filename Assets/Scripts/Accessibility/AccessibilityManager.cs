using System;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance;

    [Header("Setup Screen Toggles")]
    public Toggle visualAssistanceToggle;
    public Toggle simplifiedControlsToggle;
    public Toggle rememberSettingsToggle;

    [Header("Current Accessibility Settings")]
    public bool visualAssistance;
    public bool simplifiedControls;
    public bool rememberSettings;

    // =========================================================
    // EVENT
    // =========================================================

    public static event Action AccessibilitySettingsChanged;

    // =========================================================
    // PLAYER PREFS KEYS
    // =========================================================

    private const string VisualKey =
        "Accessibility_VisualAssistance";

    private const string ControlsKey =
        "Accessibility_SimplifiedControls";

    private const string RememberKey =
        "Accessibility_RememberSettings";

    // Compatibility keys kept so existing systems do not break.
    private const string InteractionKey =
        "Accessibility_LargeInteractionZones";

    private const string AudioKey =
        "Accessibility_AudioFeedback";

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadSavedSettings();
    }

    private void Start()
    {
        SyncToggles();
    }

    // =========================================================
    // LOAD SETTINGS
    // =========================================================

    private void LoadSavedSettings()
    {
        bool shouldRemember =
            PlayerPrefs.GetInt(RememberKey, 0) == 1;

        if (shouldRemember)
        {
            visualAssistance =
                PlayerPrefs.GetInt(VisualKey, 0) == 1;

            simplifiedControls =
                PlayerPrefs.GetInt(ControlsKey, 0) == 1;

            rememberSettings = true;
        }
        else
        {
            visualAssistance = false;
            simplifiedControls = false;
            rememberSettings = false;
        }
    }

    // =========================================================
    // APPLY SETTINGS
    // =========================================================

    public void ApplySettings()
    {
        visualAssistance =
            visualAssistanceToggle != null &&
            visualAssistanceToggle.isOn;

        simplifiedControls =
            simplifiedControlsToggle != null &&
            simplifiedControlsToggle.isOn;

        rememberSettings =
            rememberSettingsToggle != null &&
            rememberSettingsToggle.isOn;

        if (rememberSettings)
        {
            PlayerPrefs.SetInt(
                VisualKey,
                visualAssistance ? 1 : 0
            );

            PlayerPrefs.SetInt(
                ControlsKey,
                simplifiedControls ? 1 : 0
            );

            PlayerPrefs.SetInt(
                RememberKey,
                1
            );

            // Large interaction zones follow
            // Simplified Controls.
            PlayerPrefs.SetInt(
                InteractionKey,
                simplifiedControls ? 1 : 0
            );

            // Audio feedback is enabled by default.
            PlayerPrefs.SetInt(
                AudioKey,
                1
            );

            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.DeleteKey(VisualKey);
            PlayerPrefs.DeleteKey(InteractionKey);
            PlayerPrefs.DeleteKey(ControlsKey);
            PlayerPrefs.DeleteKey(AudioKey);
            PlayerPrefs.DeleteKey(RememberKey);

            PlayerPrefs.Save();
        }

        AccessibilitySettingsChanged?.Invoke();

        Debug.Log(
            "===== ACCESSIBILITY SETTINGS APPLIED =====\n" +
            "Visual Assistance = " + visualAssistance + "\n" +
            "Simplified Controls = " + simplifiedControls + "\n" +
            "Large Interaction Zones = " +
                IsLargeInteractionZonesEnabled() + "\n" +
            "Audio Feedback = " +
                IsAudioFeedbackEnabled() + "\n" +
            "Remember Settings = " + rememberSettings
        );
    }

    // =========================================================
    // SYNCHRONIZE UI
    // =========================================================

    private void SyncToggles()
    {
        if (visualAssistanceToggle != null)
        {
            visualAssistanceToggle.isOn =
                visualAssistance;
        }

        if (simplifiedControlsToggle != null)
        {
            simplifiedControlsToggle.isOn =
                simplifiedControls;
        }

        if (rememberSettingsToggle != null)
        {
            rememberSettingsToggle.isOn =
                rememberSettings;
        }
    }

    // =========================================================
    // PUBLIC ACCESSIBILITY CHECKS
    // =========================================================

    public bool IsVisualAssistanceEnabled()
    {
        return visualAssistance;
    }

    public bool IsSimplifiedControlsEnabled()
    {
        return simplifiedControls;
    }

    public bool IsRememberSettingsEnabled()
    {
        return rememberSettings;
    }

    // =========================================================
    // LARGE INTERACTION ZONES
    // =========================================================
    //
    // Your current Accessibility Setup screen does not have
    // a separate Large Interaction Zones toggle.
    //
    // Therefore, Large Interaction Zones are tied to
    // Simplified Controls.
    //
    // This fixes the Level 2 accessibility scripts without
    // adding another UI option.

    public bool IsLargeInteractionZonesEnabled()
    {
        return simplifiedControls;
    }

    // =========================================================
    // AUDIO FEEDBACK
    // =========================================================
    //
    // Your current setup screen does not have a separate
    // Audio Feedback toggle.
    //
    // Audio feedback therefore remains enabled.

    public bool IsAudioFeedbackEnabled()
    {
        return true;
    }
}