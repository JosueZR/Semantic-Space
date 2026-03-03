using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq; // Necesario para buscar Minimos y Maximos

public class SemanticSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreArchivo = "output_file";
    public GameObject prefabPunto;
    public float tamanoDelCubo = 10.0f; // El tamaño final de la nube (10x10x10)

    // Listas para guardar los datos antes de dibujarlos
    private List<Vector3> posicionesCrudas = new List<Vector3>();
    private List<DataInfo> datosPuntos = new List<DataInfo>();

    struct DataInfo
    {
        public string formula;
        public string generacion;
        public float score;
    }

    void Start()
    {
        ProcesarDatos();
    }

    void ProcesarDatos()
    {
        TextAsset archivoCSV = Resources.Load<TextAsset>(nombreArchivo);
        string contenido = archivoCSV != null ? archivoCSV.text : "";

        if (contenido == "")
        {
            string path = Application.dataPath + "/" + nombreArchivo + ".csv";
            if (File.Exists(path)) contenido = File.ReadAllText(path);
            else return;
        }

        string[] lineas = contenido.Split('\n');

        // 1. PRIMER PASO: LEER TODO Y GUARDARLO EN MEMORIA
        for (int i = 1; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea)) continue;
            string[] v = linea.Split(',');

            if (v.Length > 12)
            {
                try
                {
                    // Leemos los datos crudos
                    float x = float.Parse(v[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(v[1], CultureInfo.InvariantCulture);
                    float z = float.Parse(v[2], CultureInfo.InvariantCulture); // Etiqueta

                    posicionesCrudas.Add(new Vector3(x, y, z));

                    DataInfo info = new DataInfo();
                    info.generacion = v[2];
                    info.score = float.Parse(v[7], CultureInfo.InvariantCulture);
                    info.formula = v[12];
                    datosPuntos.Add(info);
                }
                catch { }
            }
        }

        // 2. SEGUNDO PASO: ENCONTRAR LIMITES (MIN y MAX)
        if (posicionesCrudas.Count == 0) return;

        float minX = posicionesCrudas.Min(p => p.x); float maxX = posicionesCrudas.Max(p => p.x);
        float minY = posicionesCrudas.Min(p => p.y); float maxY = posicionesCrudas.Max(p => p.y);
        float minZ = posicionesCrudas.Min(p => p.z); float maxZ = posicionesCrudas.Max(p => p.z);

        // 3. TERCER PASO: DIBUJAR NORMALIZADO (Ajustar todo entre 0 y 1)
        for (int i = 0; i < posicionesCrudas.Count; i++)
        {
            Vector3 crudo = posicionesCrudas[i];

            // Matemáticas para convertir cualquier número a un rango 0-1
            float nX = Mathf.InverseLerp(minX, maxX, crudo.x);
            float nY = Mathf.InverseLerp(minY, maxY, crudo.y);
            float nZ = Mathf.InverseLerp(minZ, maxZ, crudo.z);

            // Expandir al tamaño del cubo deseado y centrar
            Vector3 posFinal = new Vector3(
                (nX - 0.5f) * tamanoDelCubo,
                (nY - 0.5f) * tamanoDelCubo,
                (nZ - 0.5f) * tamanoDelCubo
            );

            GameObject punto = Instantiate(prefabPunto, posFinal, Quaternion.identity, this.transform);
            punto.name = "Dato_" + i;

            // Pasar datos al script visual
            if (punto.TryGetComponent(out DataPoint dp))
            {
                dp.ConfigurarDatos(datosPuntos[i].formula, datosPuntos[i].generacion, datosPuntos[i].score, false);
            }
        }
    }
}