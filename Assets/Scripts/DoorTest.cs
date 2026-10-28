using UnityEngine;
using UnityEngine.InputSystem;

public class DoorTest : MonoBehaviour
{
    [SerializeField] private DoorController door;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.tKey.wasPressedThisFrame)
        {
            door.ToggleDoor();
        }
    }
}