using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercice2 : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    List<Vector3> vertices;
    List<int> triangles;

    public Material material;

    public float rayon = 5f;
    public int nbParalleles = 5;
    public int nbMeridiens = 5;

    void createSphere()
    {
        vertices = new List<Vector3>();

        // Ajouter le sommet du pôle Nord
        vertices.Add(new Vector3(0, rayon, 0));

        // Ajouter les sommets des parallèles
        for (int p = 1; p < nbParalleles; p++) // On ignore les pôles pour l'instant
        {
            float phi = Mathf.PI * p / nbParalleles; // angle de latitude
            float y = Mathf.Cos(phi) * rayon; // hauteur du parallèle
            float rSinPhi = Mathf.Sin(phi) * rayon; // rayon de chaque parallèle

            for (int m = 0; m < nbMeridiens; m++) // Pour chaque méridien
            {
                float theta = 2 * Mathf.PI * m / nbMeridiens; // angle de longitude
                float x = rSinPhi * Mathf.Cos(theta); // Coordonnée x
                float z = rSinPhi * Mathf.Sin(theta); // Coordonnée z

                vertices.Add(new Vector3(x, y, z));
            }
        }

        // Ajouter le sommet du pôle Sud
        vertices.Add(new Vector3(0, -rayon, 0));

        mesh.vertices = vertices.ToArray();
        
        triangles = new List<int>();

        // Ajouter les triangles pour connecter les sommets
        // Connecter le pôle Nord avec le premier parallèle
        for (int m = 0; m < nbMeridiens; m++)
        {
            int nextMeridien = (m + 1) % nbMeridiens;
            triangles.Add(0); // Le pôle Nord est le premier sommet
            triangles.Add(nextMeridien + 1);
            triangles.Add(m + 1);
        }

        // Connecter les parallèles entre eux
        for (int p = 0; p < nbParalleles - 2; p++)
        {
            for (int m = 0; m < nbMeridiens; m++)
            {
                int current = m + p * nbMeridiens + 1;
                int next = (m + 1) % nbMeridiens + p * nbMeridiens + 1;
                int currentAbove = current + nbMeridiens;
                int nextAbove = next + nbMeridiens;

                // Triangle 1
                triangles.Add(current);
                triangles.Add(currentAbove);
                triangles.Add(next);

                // Triangle 2
                triangles.Add(next);
                triangles.Add(currentAbove);
                triangles.Add(nextAbove);
            }
        }

        // Connecter le dernier parallèle avec le pôle Sud
        int poleSudIndex = vertices.Count - 1;
        int lastParallelStartIndex = vertices.Count - 1 - nbMeridiens;

        for (int m = 0; m < nbMeridiens; m++)
        {
            int nextMeridien = (m + 1) % nbMeridiens;
            triangles.Add(lastParallelStartIndex + nextMeridien);
            triangles.Add(lastParallelStartIndex + m);
            triangles.Add(poleSudIndex);
        }

        mesh.triangles = triangles.ToArray();

        // Recalculer les normales pour l'éclairage
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

        //createSphere();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
