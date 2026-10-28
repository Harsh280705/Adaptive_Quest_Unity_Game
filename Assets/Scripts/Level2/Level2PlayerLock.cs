using UnityEngine;

public class Level2PlayerLock : MonoBehaviour
{
    [Header("Level 2 Player")]
    [SerializeField] private GameObject player;

    private PlayerController playerController;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Level2PlayerLock: Player is not assigned.");
            return;
        }

        playerController = player.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("Level2PlayerLock: PlayerController not found on Player.");
            return;
        }

        // Completely stop player movement in Level 2.
        playerController.enabled = false;

        // Stop any walking/footstep audio attached to Player.
        AudioSource[] audioSources = player.GetComponentsInChildren<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
            audioSource.enabled = false;
        }

        Debug.Log("LEVEL 2: Player movement and walking audio locked.");
    }
}