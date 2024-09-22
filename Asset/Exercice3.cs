using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercice3 : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    List<Vector3> vertices;
    List<int> triangles;

    public Material material;

    public float rayon = 5;
    public float hauteur = 5;
    public float hauteurTronquage = 0;
    public int nbMeridians = 16;

    void createCone()
    {
        // Si la hauteur tronquée est superieur ou egal à la hauteur totale ou inferieur ou egale à 0, le cône est complet.
        bool isTronque = hauteurTronquage < hauteur && hauteurTronquage > 0;
        // Calculer les rayons en fonction de si le cône est tronqué ou non
        float rayonHaut = isTronque ? (rayon * hauteurTronquage / hauteur) : 0;

        vertices = new List<Vector3>();
        // Ajouter les sommets pour la base du cône
        float angle = 0;
        float angleStep = 2 * Mathf.PI / nbMeridians;

        for (int i = 0; i < nbMeridians; i++)
        {
            float x = rayon * Mathf.Cos(angle);
            float z = rayon * Mathf.Sin(angle);
            vertices.Add(new Vector3(x, 0, z)); // Sommets de la base
            angle += angleStep;
        }

        // Ajouter les sommets pour la base supérieure si le cône est tronqué
        if (isTronque)
        {
            angle = 0;
            for (int i = 0; i < nbMeridians; i++)
            {
                float x = rayonHaut * Mathf.Cos(angle);
                float z = rayonHaut * Mathf.Sin(angle);
                vertices.Add(new Vector3(x, hauteurTronquage, z)); // Sommets du tronc
                angle += angleStep;
            }
        }
        else
        {
            // Si le cône n'est pas tronqué, ajouter le sommet au sommet du cône
            vertices.Add(new Vector3(0, hauteur, 0));
        }
        mesh.vertices = vertices.ToArray();

        
        triangles = new List<int>();
        int sommetIndex = vertices.Count - 2; // Sommet du cône
        // Ajouter les triangles pour la base du cône
        for (int i = 0; i < nbMeridians; i++)
        {
            int nextIndex = (i + 1) % nbMeridians;

            triangles.Add(i);
            triangles.Add(nextIndex);
            triangles.Add(sommetIndex);
        }
        if (isTronque) // Ajouter les triangles pour le sommet du tronc supérieur (couvercle du cône tronqué)
        {
            int centreTop = vertices.Count - 1; // Centre du tronc supérieur
            for (int i = 0; i < nbMeridians; i++)
            {
                int nextIndex = (i + 1) % nbMeridians;
                triangles.Add(nbMeridians + nextIndex); 
                triangles.Add(nbMeridians + i);         
                triangles.Add(centreTop);               
            }
        }

        // Ajouter les triangles pour les faces latérales du cône
        if (isTronque)
        {
            for (int i = 0; i < nbMeridians; i++)
            {
                int nextIndex = (i + 1) % nbMeridians;

                // Triangle entre la base et le tronçon supérieur, inverser l'ordre pour orienter vers l'extérieur
                triangles.Add(nbMeridians + i);            // Sommet tronqué actuel
                triangles.Add(nbMeridians + nextIndex);    // Sommet tronqué suivant
                triangles.Add(i);                          // Sommet base

                triangles.Add(nbMeridians + nextIndex);    // Sommet tronqué suivant
                triangles.Add(nextIndex);                  // Sommet base suivant
                triangles.Add(i);                          // Sommet base
            }

        }
        else
        {
            int centreBase = vertices.Count - 1;
            // Pour un cône complet, ajouter les triangles pour chaque face
            for (int i = 0; i < nbMeridians; i++)
            {
                int nextIndex = (i + 1) % nbMeridians;
                triangles.Add(i); 
                triangles.Add(centreBase); 
                triangles.Add(nextIndex);
            }
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

        //createCone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
