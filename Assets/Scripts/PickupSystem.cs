using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [Header("World Crystal")]
    [SerializeField] private GameObject crystalObject;

    [Header("Held Crystal")]
    [SerializeField] private GameObject heldCrystal;

    [Header("Pickup Audio")]
    [SerializeField] private AudioSource pickupAudio;

    private bool hasCrystal = false;

    public bool HasCrystal
    {
        get { return hasCrystal; }
    }

    private void Start()
    {
        if (heldCrystal != null)
        {
            heldCrystal.SetActive(false);
        }
    }

    public void CollectCrystal()
    {
        if (hasCrystal)
            return;

        hasCrystal = true;

        // Remove crystal from the world
        if (crystalObject != null)
        {
            crystalObject.SetActive(false);
        }

        // Show crystal in player's hand
        if (heldCrystal != null)
        {
            heldCrystal.SetActive(true);
        }

        // Play pickup sound
        if (pickupAudio != null)
        {
            pickupAudio.Play();
        }

        Debug.Log("Access Crystal collected!");
    }
}