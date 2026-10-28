using UnityEngine;

public class TerminalDisplayController : MonoBehaviour
{
    [Header("Terminal Images")]
    [SerializeField] private GameObject instructionImage;
    [SerializeField] private GameObject acceptedImage;

    private void Start()
    {
        ShowInstruction();
    }

    public void ShowInstruction()
    {
        if (instructionImage != null)
            instructionImage.SetActive(true);

        if (acceptedImage != null)
            acceptedImage.SetActive(false);
    }

    public void ShowAccepted()
    {
        if (instructionImage != null)
            instructionImage.SetActive(false);

        if (acceptedImage != null)
            acceptedImage.SetActive(true);
    }
}