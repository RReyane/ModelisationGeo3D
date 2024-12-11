using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class HermiteCurve : MonoBehaviour
{
    public Vector3 P0 = new Vector3(0, 0, 0);
    public Vector3 P1 = new Vector3(5, 5, 0);
    public Vector3 V0 = new Vector3(1, 2, 0);
    public Vector3 V1 = new Vector3(-1, 2, 0);

    public int numPoints = 50; // Nombre de points pour la courbe

    private LineRenderer lineRenderer;

    void Start()
    {
        InitializeLineRenderer();
        DrawHermiteCurve(P0, P1, V0, V1);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Mettre à jour la courbe en temps réel dans l'éditeur
            InitializeLineRenderer();
            DrawHermiteCurve(P0, P1, V0, V1);
        }

        // Dessiner des gizmos pour visualiser et manipuler les points et tangentes
        Gizmos.color = Color.red;
        P0 = Handles.PositionHandle(transform.position + P0, Quaternion.identity) - transform.position;
        Gizmos.color = Color.blue;
        P1 = Handles.PositionHandle(transform.position + P1, Quaternion.identity) - transform.position;

        Gizmos.color = Color.green;
        V0 = Handles.PositionHandle(transform.position + P0 + V0, Quaternion.identity) - (transform.position + P0);
        Gizmos.color = Color.yellow;
        V1 = Handles.PositionHandle(transform.position + P1 + V1, Quaternion.identity) - (transform.position + P1);

        // Dessiner des lignes pour indiquer les tangentes
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + P0, transform.position + P0 + V0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + P1, transform.position + P1 + V1);
    }

    private void InitializeLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
        }
        lineRenderer.positionCount = numPoints;
    }

    void DrawHermiteCurve(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1)
    {
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1); // Normalisation du paramètre t entre 0 et 1
            Vector3 point = GetHermitePoint(p0, p1, v0, v1, t);
            lineRenderer.SetPosition(i, transform.position + point);
        }
    }

    Vector3 GetHermitePoint(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, float t)
    {
        float h00 = 2 * t * t * t - 3 * t * t + 1; // Base Hermite pour p0
        float h10 = t * t * t - 2 * t * t + t;     // Base Hermite pour v0
        float h01 = -2 * t * t * t + 3 * t * t;    // Base Hermite pour p1
        float h11 = t * t * t - t * t;            // Base Hermite pour v1

        return h00 * p0 + h10 * v0 + h01 * p1 + h11 * v1;
    }
}