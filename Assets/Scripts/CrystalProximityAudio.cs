using UnityEngine;

public class CrystalProximityAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource audioSource;

    [Header("Distance")]
    [SerializeField] private float startDistance = 6f;
    [SerializeField] private float fullVolumeDistance = 1f;

    [Header("Sound")]
    [SerializeField] private float maximumVolume = 0.5f;
    [SerializeField] private float fadeSpeed = 2f;

    private void Update()
    {
        if (player == null || audioSource == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Player is outside the sound range.
        if (distance > startDistance)
        {
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                0f,
                fadeSpeed * Time.deltaTime
            );

            if (audioSource.isPlaying && audioSource.volume <= 0.01f)
            {
                audioSource.Stop();
            }

            return;
        }

        // Convert distance into a 0-1 proximity value.
        float proximity = Mathf.InverseLerp(
            startDistance,
            fullVolumeDistance,
            distance
        );

        float targetVolume = proximity * maximumVolume;

        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        // Start the looping hum when entering the range.
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}