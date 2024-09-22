using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercice1 : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    List<Vector3> vertices;
    List<int> triangles;

    public Material material;

    public float rayon = 5f;
    public float hauteur = 2f;
    public int nbMeridians = 16;

    void createCylindre()
    {
        vertices = new List<Vector3>();

        float angleStep = 2 * Mathf.PI / nbMeridians; // Pas d'angle entre les méridiens

        // Créer les sommets pour le corps du cylindre
        for (int y = 0; y <= 1; y++) // Deux niveaux : haut et bas du cylindre
        {
            for (int x = 0; x < nbMeridians; x++)
            {
                float angle = x * angleStep;
                float posX = rayon * Mathf.Cos(angle);
                float posZ = rayon * Mathf.Sin(angle);
                vertices.Add(new Vector3(posX, y * hauteur, posZ)); // Niveau bas (y = 0) et haut (y = hauteur)
            }
        }

        // Ajouter les sommets pour les couvercles supérieur et inférieur
        Vector3 centerBottom = new Vector3(0, 0, 0); // Centre du bas
        Vector3 centerTop = new Vector3(0, hauteur, 0); // Centre du haut
        vertices.Add(centerBottom); // Index = nbMeridians * 2
        vertices.Add(centerTop);    // Index = nbMeridians * 2 + 1

        mesh.vertices = vertices.ToArray();

        triangles = new List<int>();

        // Créer les triangles pour le corps du cylindre
        for (int y = 0; y < 1; y++) // Une bande entre le bas et le haut
        {
            for (int x = 0; x < nbMeridians; x++)
            {
                int current = x + nbMeridians * y;
                int next = (x + 1) % nbMeridians + nbMeridians * y;
                int upperCurrent = current + nbMeridians;
                int upperNext = next + nbMeridians;

                // Triangle 1
                triangles.Add(current);
                triangles.Add(upperCurrent);
                triangles.Add(next);

                // Triangle 2
                triangles.Add(next);
                triangles.Add(upperCurrent);
                triangles.Add(upperNext);
            }
        }


        int centerBottomIndex = nbMeridians * 2;
        int centerTopIndex = nbMeridians * 2 + 1;

        // Créer les triangles pour le couvercle inférieur
        for (int x = 0; x < nbMeridians; x++)
        {
            int current = x;
            int next = (x + 1) % nbMeridians;
            triangles.Add(next); // Centre bas
            triangles.Add(centerBottomIndex);
            triangles.Add(current);
        }

        // Créer les triangles pour le couvercle supérieur
        for (int x = 0; x < nbMeridians; x++)
        {
            int current = x + nbMeridians;
            int next = (x + 1) % nbMeridians + nbMeridians;
            triangles.Add(current); // Centre haut
            triangles.Add(centerTopIndex);
            triangles.Add(next);
        }

        mesh.triangles = triangles.ToArray();

    }

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = material;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //createCylindre();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
