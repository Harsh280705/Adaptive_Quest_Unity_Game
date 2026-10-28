using UnityEngine;

public class AccessibilityTarget : MonoBehaviour
{
    [Header("Optional Target")]
    [SerializeField] private Transform targetPoint;

    public Transform GetTargetTransform()
    {
        if (targetPoint != null)
            return targetPoint;

        return transform;
    }
}