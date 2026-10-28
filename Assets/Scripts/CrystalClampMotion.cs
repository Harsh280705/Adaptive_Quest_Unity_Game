using UnityEngine;

public class CrystalClampMotion : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float movementAmount = 0.025f;
    [SerializeField] private float movementSpeed = 0.7f;

    [Header("Movement Direction")]
    [SerializeField] private Vector3 movementDirection;

    private Vector3 startingPosition;
    private float randomOffset;

    private void Start()
    {
        startingPosition = transform.localPosition;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float movement =
            Mathf.Sin(Time.time * movementSpeed + randomOffset)
            * movementAmount;

        transform.localPosition =
            startingPosition +
            movementDirection.normalized * movement;
    }
}