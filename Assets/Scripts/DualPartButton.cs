using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DualPartButton : MonoBehaviour
{
    [Header("Componentes del Botón")]
    public Transform[] movingParts;
    public float targetZOffset = -0.00417f; // Valor exacto en Z

    [Header("Configuración")]
    public float returnSpeed = 20f;
    public bool hapticFeedback = true;
    [Range(0, 1)] public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;

    private Vector3[] originalPositions;
    private bool isPressed = false;

    void Start()
    {
        originalPositions = new Vector3[movingParts.Length];
        for (int i = 0; i < movingParts.Length; i++)
        {
            originalPositions[i] = movingParts[i].localPosition;
        }

        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(PressButton);
        GetComponent<XRSimpleInteractable>().selectExited.AddListener(ReleaseButton);
    }

    void Update()
    {
        if (!isPressed)
        {
            for (int i = 0; i < movingParts.Length; i++)
            {
                movingParts[i].localPosition = Vector3.Lerp(
                    movingParts[i].localPosition,
                    originalPositions[i],
                    returnSpeed * Time.deltaTime
                );
            }
        }
    }

    private void PressButton(SelectEnterEventArgs args)
    {
        isPressed = true;

        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 pressedPosition = originalPositions[i];
            pressedPosition.z = originalPositions[i].z + targetZOffset;
            movingParts[i].localPosition = pressedPosition;
        }

        if (hapticFeedback && args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            if (inputInteractor.TryGetComponent(out XRController controller))
            {
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }

    private void ReleaseButton(SelectExitEventArgs args)
    {
        isPressed = false;
    }
}