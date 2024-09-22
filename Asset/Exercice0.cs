using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exercice0 : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    List<Vector3> vertices;
    List<int> triangles;

    public Material material;

    public int nbRows = 3;  // Nombre de lignes de la grille
    public int nbCols = 6;    // Nombre de colonnes de la grille
    public float taille = 1f;       // Taille d'une cellule (largeur et hauteur)

    void createTriangle() 
    {
        vertices = new List<Vector3>();

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));

        mesh.vertices = vertices.ToArray();

        triangles = new List<int>();

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        mesh.triangles = triangles.ToArray();
    }

    void createRectangle()
    {
        vertices = new List<Vector3>();

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));

        mesh.vertices = vertices.ToArray();

        triangles = new List<int>();

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(2);

        mesh.triangles = triangles.ToArray();
    }

    void createPlan()
    {
        nbRows++;
        nbCols++;

        vertices = new List<Vector3>();

        for (int y = 0; y < nbRows; y++)
        {
            for (int x = 0; x < nbCols; x++)
            {
                vertices.Add(new Vector3(x * taille, y * taille, 0));

            }
        }
            mesh.vertices = vertices.ToArray();

            triangles = new List<int>();


            int ceil, floor;

            for (int k = 0; k < nbRows - 1; k++)
            {
                ceil = nbCols * (k + 1);
                floor = nbCols * k;
                for (int i = 0; i < nbCols - 1; i++)
                {
                    triangles.Add(floor);
                    triangles.Add(ceil);
                    triangles.Add(floor + 1);

                    triangles.Add(ceil);
                    triangles.Add(ceil + 1);
                    triangles.Add(floor + 1);

                    ceil++; floor++;
                }
            }

            mesh.triangles = triangles.ToArray();

            // Recalculer les normales pour que l'éclairage soit correct
            mesh.RecalculateNormals();
    }


    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = material;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        createPlan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}