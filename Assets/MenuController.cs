using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public SemanticSpawner miSpawner; // Arrastra aquí tu objeto Spawner

    [Header("UI Elementos")]
    public Button btnPlay;
    public Button btnShowAll;
    public Slider sliderGeneraciones;
    public TMP_Text textoSlider; // Para mostrar el número al lado

    void Start()
    {
        // Conectamos los botones a las funciones
        btnPlay.onClick.AddListener(AlPulsarPlay);
        sliderGeneraciones.onValueChanged.AddListener(AlMoverSlider);
    }

    void AlPulsarPlay()
    {
        Debug.Log("▶️ Play pulsado");
        // Aquí llamarías a la función de tu Spawner, por ejemplo:
        // miSpawner.IniciarSimulacion(); 
    }

    void AlMoverSlider(float valor)
    {
        textoSlider.text = valor.ToString("0");
        // miSpawner.MostrarHastaGeneracion((int)valor);
    }
}