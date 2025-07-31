using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DualPartButton : MonoBehaviour
{
    [Header("Componentes del Bot�n")]
    public Transform[] movingParts;  // Asignar pCylinder96 y pCylinder97
    public float pressDepth = 0.05f; // Distancia que baja el bot�n (en metros)
    public float returnSpeed = 10f;  // Velocidad de retorno

    [Header("Configuraci�n de Interacci�n")]
    public bool stayPressed = false; // Si el bot�n queda presionado o vuelve solo
    public bool hapticFeedback = true; // Vibraci�n al presionar
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

        // Configurar eventos de interacci�n
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(PressButton);

        if (!stayPressed)
        {
            interactable.selectExited.AddListener(ReleaseButton);
        }
    }

    void Update()
    {
        // Animaci�n de retorno si no est� presionado
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

        // Mover partes a posici�n presionada (eje Z negativo)
        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 pressedPosition = originalPositions[i];
            pressedPosition.z -= pressDepth;
            movingParts[i].localPosition = pressedPosition;
        }

        // Retroceso h�ptico (solo si est� habilitado)
        if (hapticFeedback && args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            inputInteractor.xrController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }

        // Ejecutar acci�n del bot�n
        ExecuteButtonAction();
    }

    private void ReleaseButton(SelectExitEventArgs args)
    {
        isPressed = false;
    }

    private void ExecuteButtonAction()
    {
        string buttonName = gameObject.name.ToLower();

        if (buttonName.Contains("on")) // Si es el bot�n "ON"
        {
            Debug.Log("Bot�n ON activado");
            // Aqu� tu l�gica para el bot�n ON (ej: encender luz)
        }
        else if (buttonName.Contains("off")) // Si es el bot�n "OFF"
        {
            Debug.Log("Bot�n OFF activado");
            // Aqu� tu l�gica para el bot�n OFF (ej: apagar luz)
        }
    }

    // M�todo p�blico para controlar el bot�n desde otros scripts
    public void ToggleButton(bool pressed)
    {
        if (pressed) PressButton(null);
        else ReleaseButton(null);
    }
}