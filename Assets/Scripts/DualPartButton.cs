using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DualPartButton : MonoBehaviour
{
    [Header("Componentes del Botón")]
    public Transform[] movingParts;  // Asignar pCylinder96 y pCylinder97
    public float pressDepth = 0.05f; // Distancia que baja el botón (en metros)
    public float returnSpeed = 10f;  // Velocidad de retorno

    [Header("Configuración de Interacción")]
    public bool stayPressed = false; // Si el botón queda presionado o vuelve solo
    public bool hapticFeedback = true; // Vibración al presionar
    public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;

    private Vector3[] originalPositions;
    private bool isPressed = false;

    void Start()
    {
        // Guardar posiciones originales
        originalPositions = new Vector3[movingParts.Length];
        for (int i = 0; i < movingParts.Length; i++)
        {
            originalPositions[i] = movingParts[i].localPosition;
        }

        // Configurar eventos de interacción
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(PressButton);

        if (!stayPressed)
        {
            interactable.selectExited.AddListener(ReleaseButton);
        }
    }

    void Update()
    {
        // Animación de retorno si no está presionado
        if (!isPressed && !stayPressed)
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

        // Mover partes a posición presionada (eje Z negativo)
        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 pressedPosition = originalPositions[i];
            pressedPosition.z -= pressDepth;
            movingParts[i].localPosition = pressedPosition;
        }

        // Retroceso háptico (solo si está habilitado)
        if (hapticFeedback && args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            inputInteractor.xrController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }

        // Ejecutar acción del botón
        ExecuteButtonAction();
    }

    private void ReleaseButton(SelectExitEventArgs args)
    {
        isPressed = false;
    }

    private void ExecuteButtonAction()
    {
        string buttonName = gameObject.name.ToLower();

        if (buttonName.Contains("on")) // Si es el botón "ON"
        {
            Debug.Log("Botón ON activado");
            // Aquí tu lógica para el botón ON (ej: encender luz)
        }
        else if (buttonName.Contains("off")) // Si es el botón "OFF"
        {
            Debug.Log("Botón OFF activado");
            // Aquí tu lógica para el botón OFF (ej: apagar luz)
        }
    }

    // Método público para controlar el botón desde otros scripts
    public void ToggleButton(bool pressed)
    {
        if (pressed) PressButton(null);
        else ReleaseButton(null);
    }
}