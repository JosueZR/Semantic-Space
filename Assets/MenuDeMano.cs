using UnityEngine;
using UnityEngine.InputSystem;

public class MenuDeMano : MonoBehaviour
{
    [Header("Arrastra aquí tu Canvas")]
    public GameObject miMenu;

    [Header("Configuración del Botón")]
    public InputActionProperty botonParaAbrir;

    [Header("Opciones")]
    public bool queMeSiga = true;
    public float distancia = 0.5f; // Lo bajé a 0.5 para que esté cerquita

    // --- ESTA ES LA PARTE NUEVA QUE FALTABA ---
    void OnEnable()
    {
        botonParaAbrir.action.Enable(); // ¡Despierta botón!
    }

    void OnDisable()
    {
        botonParaAbrir.action.Disable();
    }
    // -------------------------------------------

    void Update()
    {
        if (botonParaAbrir.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        bool estadoActual = miMenu.activeSelf;
        miMenu.SetActive(!estadoActual);

        if (!estadoActual && queMeSiga)
        {
            PosicionarFrenteCamara();
        }
    }

    void PosicionarFrenteCamara()
    {
        Transform cabeza = Camera.main.transform;
        miMenu.transform.position = cabeza.position + (cabeza.forward * distancia);

        Vector3 direccion = miMenu.transform.position - cabeza.position;
        direccion.y = 0;
        miMenu.transform.rotation = Quaternion.LookRotation(direccion);
    }
}