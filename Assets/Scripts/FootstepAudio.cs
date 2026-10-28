using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private void Update()
    {
        if (audioSource == null)
            return;

        bool isWalking = false;

        if (Keyboard.current != null)
        {
            isWalking =
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.dKey.isPressed;
        }

        if (isWalking)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}