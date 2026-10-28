using UnityEngine;

public class Level2Accessibility : MonoBehaviour
{
    [Header("LEVEL 2 CARDS")]
    [SerializeField] private Level2SequenceCard[] cards;

    [Header("LARGE INTERACTION ZONES")]
    [SerializeField] private float colliderScale = 1.5f;

    private Vector3[] originalColliderSizes;
    private Vector3[] originalColliderCenters;

    private void Start()
    {
        StoreOriginalColliderData();
        ApplyAccessibility();
    }

    private void StoreOriginalColliderData()
    {
        if (cards == null)
            return;

        originalColliderSizes = new Vector3[cards.Length];
        originalColliderCenters = new Vector3[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
                continue;

            BoxCollider collider =
                cards[i].GetComponent<BoxCollider>();

            if (collider == null)
                continue;

            originalColliderSizes[i] = collider.size;
            originalColliderCenters[i] = collider.center;
        }
    }

    public void ApplyAccessibility()
    {
        if (AccessibilityManager.Instance == null)
            return;

        bool largeZones =
            AccessibilityManager.Instance
            .IsLargeInteractionZonesEnabled();

        if (cards == null)
            return;

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
                continue;

            BoxCollider collider =
                cards[i].GetComponent<BoxCollider>();

            if (collider == null)
                continue;

            if (largeZones)
            {
                collider.size =
                    originalColliderSizes[i] * colliderScale;
            }
            else
            {
                collider.size =
                    originalColliderSizes[i];

                collider.center =
                    originalColliderCenters[i];
            }
        }
    }
}