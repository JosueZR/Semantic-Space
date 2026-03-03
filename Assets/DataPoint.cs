using UnityEngine;
using TMPro;

public class DataPoint : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject canvasPanel;
    public TMP_Text formulaText;
    public TMP_Text statsText;
    public Renderer miRenderer;

    void Awake()
    {
        // 1. Buscando Canvas
        if (canvasPanel == null)
            canvasPanel = GetComponentInChildren<Canvas>()?.gameObject;

        if (canvasPanel == null) Debug.LogError("❌ ERROR CRÍTICO: ¡No encuentro el Canvas hijo en " + gameObject.name + "!");
        else Debug.Log("✅ Canvas encontrado en " + gameObject.name);

        // 2. Buscando Textos
        if (formulaText == null || statsText == null)
        {
            TMP_Text[] textos = GetComponentsInChildren<TMP_Text>(true);
            Debug.Log("🔍 Buscando textos... Encontrados: " + textos.Length);

            if (textos.Length >= 1) formulaText = textos[0];
            if (textos.Length >= 2) statsText = textos[1];
        }

        if (formulaText == null) Debug.LogError("❌ ERROR: No encuentro el primer Texto (Formula)");
        if (statsText == null) Debug.LogError("❌ ERROR: No encuentro el segundo Texto (Stats)");

        // 3. Renderer
        if (miRenderer == null) miRenderer = GetComponent<Renderer>();

        // ⚠️ COMENTADO PARA QUE NO SE OCULTE Y PODAMOS VERLO
        // if (canvasPanel != null) canvasPanel.SetActive(false);
    }

    public void ConfigurarDatos(string formula, string gen, float score, bool isBest)
    {
        Debug.Log("📩 Recibiendo datos: " + formula); // CONFIRMACIÓN DE QUE LLEGAN DATOS

        if (formulaText != null) formulaText.text = formula;
        else Debug.LogError("❌ Intente escribir la fórmula pero formulaText es NULL");

        if (statsText != null) statsText.text = $"Gen: {gen} | Score: {score.ToString("F3")}";
    }
}