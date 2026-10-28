using UnityEngine;

public static class AccessibilityFeedback
{
    private static GameObject audioObject;
    private static AudioSource audioSource;

    private static AudioClip interactionClip;
    private static AudioClip blockedClip;
    private static AudioClip navigationClip;
    private static AudioClip successClip;
    private static AudioClip warningClip;
    private static AudioClip errorClip;
    private static AudioClip confirmationClip;

    private static bool initialized;

    private static float lastFeedbackTime = -10f;

    private const float MinimumFeedbackInterval = 0.06f;

    // =========================================================
    // AUDIO ENABLE CHECK
    // =========================================================

    private static bool AudioEnabled()
    {
        if (AccessibilityManager.Instance == null)
            return true;

        return AccessibilityManager.Instance
            .IsAudioFeedbackEnabled();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    private static void Initialize()
    {
        if (initialized &&
            audioObject != null &&
            audioSource != null)
        {
            return;
        }

        audioObject =
            GameObject.Find(
                "AccessibilityAudioFeedback"
            );

        if (audioObject == null)
        {
            audioObject =
                new GameObject(
                    "AccessibilityAudioFeedback"
                );

            Object.DontDestroyOnLoad(audioObject);
        }

        audioSource =
            audioObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                audioObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.mute = false;

        interactionClip =
            CreateTone(
                "Accessibility_Interaction",
                700f,
                0.14f,
                0.65f
            );

        blockedClip =
            CreateTwoTone(
                "Accessibility_Blocked",
                260f,
                180f,
                0.22f
            );

        navigationClip =
            CreateTone(
                "Accessibility_Navigation",
                520f,
                0.11f,
                0.60f
            );

        successClip =
            CreateTwoTone(
                "Accessibility_Success",
                650f,
                950f,
                0.28f
            );

        warningClip =
            CreateTwoTone(
                "Accessibility_Warning",
                440f,
                330f,
                0.25f
            );

        errorClip =
            CreateTwoTone(
                "Accessibility_Error",
                220f,
                150f,
                0.28f
            );

        confirmationClip =
            CreateTwoTone(
                "Accessibility_Confirmation",
                700f,
                1050f,
                0.28f
            );

        initialized = true;
    }

    // =========================================================
    // PUBLIC FEEDBACK
    // =========================================================

    public static void PlayInteractionFeedback()
    {
        PlayClip(
            interactionClip,
            1f
        );
    }

    public static void PlayBlockedFeedback()
    {
        PlayClip(
            blockedClip,
            1f
        );
    }

    public static void PlayNavigationFeedback()
    {
        PlayClip(
            navigationClip,
            0.9f
        );
    }

    public static void PlaySuccessFeedback()
    {
        PlayClip(
            successClip,
            1f
        );
    }

    public static void PlayWarningFeedback()
    {
        PlayClip(
            warningClip,
            1f
        );
    }

    public static void PlayErrorFeedback()
    {
        PlayClip(
            errorClip,
            1f
        );
    }

    public static void PlayConfirmationFeedback()
    {
        PlayClip(
            confirmationClip,
            1f
        );
    }

    // =========================================================
    // PLAY
    // =========================================================

    private static void PlayClip(
        AudioClip clip,
        float volume)
    {
        if (!AudioEnabled())
            return;

        Initialize();

        if (audioSource == null ||
            clip == null)
        {
            return;
        }

        if (Time.unscaledTime -
            lastFeedbackTime <
            MinimumFeedbackInterval)
        {
            return;
        }

        lastFeedbackTime =
            Time.unscaledTime;

        audioSource.PlayOneShot(
            clip,
            Mathf.Clamp01(volume)
        );
    }

    // =========================================================
    // SINGLE TONE
    // =========================================================

    private static AudioClip CreateTone(
        string name,
        float frequency,
        float duration,
        float volume)
    {
        int sampleRate =
            AudioSettings.outputSampleRate;

        int samplesCount =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    sampleRate * duration
                )
            );

        float[] samples =
            new float[samplesCount];

        for (int i = 0;
             i < samplesCount;
             i++)
        {
            float time =
                (float)i /
                sampleRate;

            float envelope =
                GetEnvelope(
                    time,
                    duration
                );

            samples[i] =
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    time
                ) *
                envelope *
                volume;
        }

        AudioClip clip =
            AudioClip.Create(
                name,
                samplesCount,
                1,
                sampleRate,
                false
            );

        clip.SetData(
            samples,
            0
        );

        return clip;
    }

    // =========================================================
    // TWO TONE
    // =========================================================

    private static AudioClip CreateTwoTone(
        string name,
        float firstFrequency,
        float secondFrequency,
        float duration)
    {
        int sampleRate =
            AudioSettings.outputSampleRate;

        int samplesCount =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    sampleRate * duration
                )
            );

        float[] samples =
            new float[samplesCount];

        float split =
            duration * 0.48f;

        for (int i = 0;
             i < samplesCount;
             i++)
        {
            float time =
                (float)i /
                sampleRate;

            float frequency =
                time < split
                    ? firstFrequency
                    : secondFrequency;

            float envelope =
                GetEnvelope(
                    time,
                    duration
                );

            samples[i] =
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    time
                ) *
                envelope *
                0.70f;
        }

        AudioClip clip =
            AudioClip.Create(
                name,
                samplesCount,
                1,
                sampleRate,
                false
            );

        clip.SetData(
            samples,
            0
        );

        return clip;
    }

    // =========================================================
    // ENVELOPE
    // =========================================================

    private static float GetEnvelope(
        float time,
        float duration)
    {
        float attack =
            Mathf.Clamp01(
                time / 0.015f
            );

        float release =
            Mathf.Clamp01(
                (duration - time) /
                0.04f
            );

        return Mathf.Min(
            attack,
            release
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    public static void Shutdown()
    {
        if (audioObject != null)
        {
            Object.Destroy(
                audioObject
            );
        }

        audioObject = null;
        audioSource = null;

        interactionClip = null;
        blockedClip = null;
        navigationClip = null;
        successClip = null;
        warningClip = null;
        errorClip = null;
        confirmationClip = null;

        initialized = false;
    }
}