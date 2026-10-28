using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openHeight = 3.5f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource openAudio;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    private void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            openSpeed * Time.deltaTime
        );
    }

    public void ToggleDoor()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (openAudio != null)
        {
            openAudio.Play();
        }
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}