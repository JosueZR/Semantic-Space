using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PanelDatosUI : MonoBehaviour
{
    public SemanticSpawner spawner; // Arrastra el Spawner aquí

    [Header("Botones")]
    public Button btnPlay;
    public Button btnShowAll;

    [Header("Slider")]
    public Slider sliderGeneraciones;
    public TMP_InputField inputGeneracion; // La cajita blanca al lado del slider

    private Coroutine rutinaPlay;
    private bool estaReproduciendo = false;

    void Start()
    {
        // Conectar botones
        btnShowAll.onClick.AddListener(AlPulsarShowAll);
        btnPlay.onClick.AddListener(AlPulsarPlay);

        // Conectar Slider
        sliderGeneraciones.onValueChanged.AddListener(AlMoverSlider);

        // Esperamos un poquito para que el Spawner cargue el CSV y luego configuramos el Slider
        Invoke("ConfigurarSlider", 1.0f);
    }

    void ConfigurarSlider()
    {
        if (spawner.listaGeneraciones.Count > 0)
        {
            sliderGeneraciones.minValue = spawner.listaGeneraciones[0];
            sliderGeneraciones.maxValue = spawner.listaGeneraciones[spawner.listaGeneraciones.Count - 1];
            sliderGeneraciones.value = sliderGeneraciones.minValue;
        }
    }

    void AlMoverSlider(float valor)
    {
        int generacionActual = (int)valor;
        inputGeneracion.text = generacionActual.ToString(); // Actualiza el texto de la cajita
        spawner.MostrarHastaGeneracion(generacionActual);   // Enciende las esferas
    }

    void AlPulsarShowAll()
    {
        DetenerPlay();
        sliderGeneraciones.value = sliderGeneraciones.maxValue;
        spawner.MostrarTodas();
    }

    void AlPulsarPlay()
    {
        if (estaReproduciendo) DetenerPlay();
        else rutinaPlay = StartCoroutine(AnimacionPlay());
    }

    void DetenerPlay()
    {
        if (rutinaPlay != null) StopCoroutine(rutinaPlay);
        estaReproduciendo = false;
        // Opcional: Cambiar el texto/color del botón Play
    }

    IEnumerator AnimacionPlay()
    {
        estaReproduciendo = true;

        // Si ya estamos al final, volvemos a empezar
        if (sliderGeneraciones.value >= sliderGeneraciones.maxValue)
        {
            sliderGeneraciones.value = sliderGeneraciones.minValue;
        }

        // Vamos subiendo el slider poco a poco
        foreach (int gen in spawner.listaGeneraciones)
        {
            if (gen < sliderGeneraciones.value) continue; // Saltamos las que ya pasaron

            sliderGeneraciones.value = gen; // Esto moverá el slider visualmente y encenderá las esferas
            yield return new WaitForSeconds(0.5f); // Tiempo entre cada generación (medio segundo)
        }

        estaReproduciendo = false;
    }
}