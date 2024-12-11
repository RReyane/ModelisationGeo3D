using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
    public Vector3[] controlPoints = new Vector3[4] // Points de contrôle initiaux
    {
        new Vector3(-2, -2, 0),
        new Vector3(-1, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(2, -2, 0)
    };

    public int resolution = 50; // Nombre de points pour la courbe
    private int selectedPointIndex = 0; // Index du point de contrôle sélectionné

    private LineRenderer lineRenderer;

    void Start()
    {
        InitializeLineRenderer();
        DrawBezierCurve();
    }

    void Update()
    {
        HandleInput();
        DrawBezierCurve();
    }

    void OnDrawGizmos()
    {
        // Dessin des points de contrôle
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Gizmos.color = i == selectedPointIndex ? Color.blue : Color.red;
            Gizmos.DrawCube(transform.position + controlPoints[i], Vector3.one * 0.1f);
        }

        // Dessin du polygone reliant les points de contrôle
        Gizmos.color = Color.gray;
        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(transform.position + controlPoints[i], transform.position + controlPoints[i + 1]);
        }
    }

    private void InitializeLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    private void DrawBezierCurve()
    {
        Vector3[] curvePoints = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            curvePoints[i] = CalculateBezierPoint(t);
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(curvePoints);
    }

    private Vector3 CalculateBezierPoint(float t)
    {
        Vector3 point = Vector3.zero;
        int n = controlPoints.Length - 1;
        for (int i = 0; i <= n; i++)
        {
            float bernstein = Bernstein(n, i, t);
            point += bernstein * controlPoints[i];
        }
        return transform.position + point;
    }

    private float Bernstein(int n, int i, float t)
    {
        return Factorial(n) / (Factorial(i) * Factorial(n - i)) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
    }

    private int Factorial(int x)
    {
        if (x <= 1) return 1;
        return x * Factorial(x - 1);
    }

    private void HandleInput()
    {
        // Changer le point sélectionné avec les touches 0, 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha0)) selectedPointIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedPointIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedPointIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedPointIndex = 3;

        // Déplacer le point sélectionné avec Z, Q, S, D
        if (Input.GetKey(KeyCode.W)) controlPoints[selectedPointIndex] += Vector3.up * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) controlPoints[selectedPointIndex] += Vector3.right * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) controlPoints[selectedPointIndex] += Vector3.down * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) controlPoints[selectedPointIndex] += Vector3.left * Time.deltaTime;
    }
}