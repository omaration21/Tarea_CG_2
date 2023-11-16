using UnityEngine;

public class RuedasTrans : MonoBehaviour
{
    [SerializeField] private Vector3 displacement;
    [SerializeField] private float angle;
    [SerializeField] private AXIS rotationAxis;
    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] newVertices;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        
        // Comprueba si el componente MeshFilter y la malla no son nulos
        if (meshFilter != null && meshFilter.mesh != null)
        {
            // Crea una copia profunda de la malla y asigna la copia a meshFilter
            mesh = Instantiate(meshFilter.mesh);
            mesh.name = "copia_" + gameObject.name; // Opcional: dar un nuevo nombre a la malla para identificarla fácilmente
            meshFilter.mesh = mesh;
            
            // Inicializa baseVertices y newVertices con la copia de la malla
            baseVertices = mesh.vertices;
            newVertices = new Vector3[baseVertices.Length];
            System.Array.Copy(baseVertices, newVertices, baseVertices.Length);
        }
        else
        {
            Debug.LogError("MeshFilter o mesh es nulo en " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mesh != null)
        {
            DoTransform();
        }
        else
        {
            Debug.LogError("La malla es nula, no se puede realizar la transformación.");
        }
    }
    void DoTransform()
    {
        // Asegurarse de que baseVertices y newVertices no son null
        if (baseVertices == null || newVertices == null)
        {
            Debug.LogError("baseVertices o newVertices no han sido inicializados.");
            return;
        }

        // Matriz de traslación en función del tiempo multiplicado por el desplazamiento
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.time, displacement.y * Time.time, displacement.z * Time.time);

        // Matriz de rotación en función del tiempo multiplicado por el ángulo y el eje de rotación
        Matrix4x4 rotate = HW_Transforms.RotateMat(angle * Time.time, rotationAxis);

        // Matriz compuesta de traslación y rotación
        Matrix4x4 composite = move * rotate;

        // Aplicar la transformación a cada vértice del objeto
        for (int i = 0; i < newVertices.Length; i++)
        {
            // Crear un vector temporal con las coordenadas del vértice original
            Vector4 temp = new Vector4(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);
            // Aplicar la transformación a cada vértice usando la matriz compuesta
            newVertices[i] = composite.MultiplyPoint3x4(temp);
        }
        
        // Reemplazar los vértices en el objeto de malla
        mesh.vertices = newVertices;

        // Recalcular las normales de la malla
        mesh.RecalculateNormals();

        // Opcional: recalcular límites si la forma de la malla ha cambiado significativamente
        mesh.RecalculateBounds();
    }
}