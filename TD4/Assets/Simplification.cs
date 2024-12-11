using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using objBuilder;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Simplification : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public Material material;

    Charge statue;

    [Range(0f,30)]
    public float tolerance;

    private Dictionary<Vector3Int, List<Vector3>> pointsParCase;
    private Dictionary<Vector3Int, Vector3> pointTransformeParCase;
    private Dictionary<Vector3, Vector3> originalToClustered;       // Lien entre anciens sommets et sommets moyens

    void Start()
    {
        

        initMesh();
    }

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            initDict();
            calcNewVertices();
            modifMesh();

        //Debug: Afficher le nombre de cellules remplies
            Debug.Log("Nombre de cellules avec des sommets: " + pointTransformeParCase.Count);
        }
    }

    // Convertit une position de sommet en coordonnée de cellule
    private Vector3Int GetCellCoord(Vector3 vertexPosition)
    {
        int x = vertexPosition.x < 0 ? Mathf.CeilToInt(vertexPosition.x / tolerance) : Mathf.FloorToInt(vertexPosition.x / tolerance);
        int y = vertexPosition.y < 0 ? Mathf.CeilToInt(vertexPosition.y / tolerance) : Mathf.FloorToInt(vertexPosition.y / tolerance);
        int z = vertexPosition.z < 0 ? Mathf.CeilToInt(vertexPosition.z / tolerance) : Mathf.FloorToInt(vertexPosition.z/ tolerance);

        return new Vector3Int(x, y, z);
    }

    void initMesh()
    {
        statue = new Charge();
        statue.Execute(gameObject);
        mesh = statue.mesh;
        meshFilter = statue.meshFilter;
        meshRenderer = statue.meshRenderer;
        meshRenderer.material = material;

        gameObject.transform.localScale = Vector3.one * 10;

        Debug.Log("Nb triangels avant = " + mesh.triangles.Length);
    }

    private void initDict()
    {
        // Initialiser le dictionnaire de cellules
        pointsParCase = new Dictionary<Vector3Int, List<Vector3>>();

        tolerance = tolerance / 100;

        // Construire la grille en assignant chaque sommet à une cellule
        foreach (Vector3 vertex in mesh.vertices)
        {
            // Convertir la position du sommet en coordonnée de cellule
            Vector3Int cellCoord = GetCellCoord(vertex);

            // Ajouter le sommet à la liste des sommets de cette cellule
            if (!pointsParCase.ContainsKey(cellCoord))
            {
                pointsParCase[cellCoord] = new List<Vector3>();
            }
            pointsParCase[cellCoord].Add(vertex);
        }
    }

    private void calcNewVertices()
    {
        pointTransformeParCase = new Dictionary<Vector3Int, Vector3>();

        foreach (var cell in pointsParCase.Keys)
        {
            Vector3 newVertex = new Vector3(0,0,0);
            foreach(Vector3 vertex in pointsParCase[cell])
            {
                newVertex += vertex;
            }
            newVertex /= pointsParCase[cell].Count;
            pointTransformeParCase.Add(cell, newVertex);
        }
    }

    private void modifMesh()
    {

        // 1. Initialiser le dictionnaire qui va associer chaque ancien sommet à un nouveau sommet
        originalToClustered = new Dictionary<Vector3, Vector3>();

        foreach (var cell in pointTransformeParCase.Keys)
        {
            foreach (Vector3 vertex in pointsParCase[cell])
            {
                originalToClustered[vertex] = pointTransformeParCase[cell];
            }
        }

        // 2. Recalculer les triangles
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // Créer une liste de nouveaux triangles après regroupement
        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        Dictionary<Vector3, int> vertexToIndex = new Dictionary<Vector3, int>();

        foreach (int index in triangles)
        {
            Vector3 originalVertex = vertices[index];
            Vector3 newVertex = originalToClustered[originalVertex];

            // Vérifier si le sommet moyen est déjà dans la liste des nouveaux sommets
            if (!vertexToIndex.ContainsKey(newVertex))
            {
                vertexToIndex[newVertex] = newVertices.Count;
                newVertices.Add(newVertex);
            }

            // Ajouter l'indice du sommet moyen dans la liste des nouveaux triangles
            newTriangles.Add(vertexToIndex[newVertex]);
        }


        // 3. Mettre à jour le mesh avec les nouveaux sommets et triangles
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        Debug.Log("Nb triangels apres = " + mesh.triangles.Length);
    }
}
