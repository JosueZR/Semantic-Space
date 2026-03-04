using UnityEngine;
using System.Collections; // Necesario para la Corrutina
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class SemanticSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreArchivo = "output_file";
    public GameObject prefabPunto;
    public float tamanoDelCubo = 10.0f;

    public Dictionary<int, List<GameObject>> esferasPorGeneracion = new Dictionary<int, List<GameObject>>();
    public List<int> listaGeneraciones = new List<int>();

    private List<Vector3> posicionesCrudas = new List<Vector3>();
    private List<DataInfo> datosPuntos = new List<DataInfo>();

    struct DataInfo
    {
        public string formula;
        public int generacion;
        public float score;
    }

    void Start()
    {
        // En lugar de procesar todo de golpe, iniciamos la carga paso a paso
        StartCoroutine(ProcesarDatosPocoAPoco());
    }

    // Esta es la función mágica que no congela tu PC
    IEnumerator ProcesarDatosPocoAPoco()
    {
        TextAsset archivoCSV = Resources.Load<TextAsset>(nombreArchivo);
        string contenido = archivoCSV != null ? archivoCSV.text : "";

        if (contenido == "")
        {
            string path = Application.dataPath + "/" + nombreArchivo + ".csv";
            if (File.Exists(path)) contenido = File.ReadAllText(path);
            else yield break;
        }

        string[] lineas = contenido.Split('\n');

        // Paso 1: Leemos el texto (esto es muy rápido, no traba la PC)
        for (int i = 1; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea)) continue;
            string[] v = linea.Split(',');

            if (v.Length > 12)
            {
                try
                {
                    float x = float.Parse(v[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(v[1], CultureInfo.InvariantCulture);
                    float z = float.Parse(v[2], CultureInfo.InvariantCulture);

                    posicionesCrudas.Add(new Vector3(x, y, z));

                    DataInfo info = new DataInfo();
                    info.generacion = (int)float.Parse(v[2], CultureInfo.InvariantCulture);
                    info.score = float.Parse(v[7], CultureInfo.InvariantCulture);
                    info.formula = v[12];
                    datosPuntos.Add(info);
                }
                catch { }
            }
        }

        if (posicionesCrudas.Count == 0) yield break;

        float minX = posicionesCrudas.Min(p => p.x); float maxX = posicionesCrudas.Max(p => p.x);
        float minY = posicionesCrudas.Min(p => p.y); float maxY = posicionesCrudas.Max(p => p.y);
        float minZ = posicionesCrudas.Min(p => p.z); float maxZ = posicionesCrudas.Max(p => p.z);

        Dictionary<int, Color> coloresGeneracion = new Dictionary<int, Color>();

        // Paso 2: Crear las esferas POCO A POCO
        for (int i = 0; i < posicionesCrudas.Count; i++)
        {
            Vector3 crudo = posicionesCrudas[i];
            float nX = Mathf.InverseLerp(minX, maxX, crudo.x);
            float nY = Mathf.InverseLerp(minY, maxY, crudo.y);
            float nZ = Mathf.InverseLerp(minZ, maxZ, crudo.z);

            Vector3 posFinal = new Vector3((nX - 0.5f) * tamanoDelCubo, (nY - 0.5f) * tamanoDelCubo, (nZ - 0.5f) * tamanoDelCubo);

            GameObject punto = Instantiate(prefabPunto, posFinal, Quaternion.identity, this.transform);
            punto.name = "Dato_" + i;

            int miGen = datosPuntos[i].generacion;

            if (!coloresGeneracion.ContainsKey(miGen)) coloresGeneracion[miGen] = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            if (punto.TryGetComponent(out Renderer renderEsfera)) renderEsfera.material.color = coloresGeneracion[miGen];
            if (punto.TryGetComponent(out DataPoint dp)) dp.ConfigurarDatos(datosPuntos[i].formula, miGen.ToString(), datosPuntos[i].score, false);

            if (!esferasPorGeneracion.ContainsKey(miGen))
            {
                esferasPorGeneracion[miGen] = new List<GameObject>();
                listaGeneraciones.Add(miGen);
            }
            esferasPorGeneracion[miGen].Add(punto);
            punto.SetActive(false);

            // EL SECRETO: Cada 100 esferas, le decimos a Unity: "Pausa un frame y dibuja el mundo"
            if (i % 100 == 0)
            {
                yield return null;
            }
        }

        // Ordenamos y mostramos la primera generación al terminar de cargar
        listaGeneraciones.Sort();
        if (listaGeneraciones.Count > 0) MostrarHastaGeneracion(listaGeneraciones[0]);

        Debug.Log("¡Carga de datos VR completada con éxito!");
    }

    public void MostrarHastaGeneracion(int topeGen)
    {
        foreach (var kvp in esferasPorGeneracion)
        {
            bool debeEstarPrendida = (kvp.Key <= topeGen);
            foreach (GameObject esfera in kvp.Value)
            {
                // 🚀 EL TRUCO ANTI-LAG:
                // Solo le decimos a Unity que cambie la esfera SI SU ESTADO ES DIFERENTE.
                // Si ya estaba apagada y debe estar apagada, la ignora y ahorramos memoria.
                if (esfera.activeSelf != debeEstarPrendida)
                {
                    esfera.SetActive(debeEstarPrendida);
                }
            }
        }
    }

    public void MostrarTodas()
    {
        if (listaGeneraciones.Count > 0)
        {
            MostrarHastaGeneracion(listaGeneraciones.Max());
        }
    }
}