using System.Collections.Generic;
using UnityEngine;

public class EnumerationSpatiale : MonoBehaviour
{
    public int gridResolution = 10;
    public GameObject cubePrefab;
    public bool intersect = false;
    public bool union = true;
    public bool difference = false;

    public float Threshold = 0f;  // Seuil pour la visibilit�
    public float toolInfluence = 50f;       // Potentiel ajout�/soustrait par l'outil
    public GameObject toolSphere;          // Sph�re Unity utilis�e comme outil

    private class Sphere
    {
        public Vector3 center;
        public float radius;

        public Sphere(Vector3 c, float r) { center = c; radius = r; }
    }
    private List<Sphere> spheresList = new List<Sphere>();

    private class CubeData
    {
        public GameObject cube;
        public float potential;

        public CubeData(GameObject cube, float initialPotential)
        {
            this.cube = cube;
            this.potential = initialPotential;
        }

        // Met � jour la visibilit� du cube selon le seuil
        public void UpdateVisibility(float threshold)
        {
            cube.SetActive(potential > threshold);
        }
    }
    private Dictionary<Vector3, CubeData> cubeDictionary = new Dictionary<Vector3, CubeData>();

    // Initialisation des sph�res
    void Start()
    {
        spheresList.Add(new Sphere(new Vector3(0, 0, 0), 1f));
        spheresList.Add(new Sphere(new Vector3(1, 1, 1), 2f));

        // G�n�ration des cubes pour chaque sph�re
        GenerateCubes();
    }

    void Update()
    {
        // Mise � jour du potentiel des cubes si la sph�re est d�plac�e
        if (toolSphere != null )
        {
            UpdatePotentialWithTool();
        }
            UpdateCubesVisibility();
    }

    // G�n�ration des cubes pour toutes les sph�res
    private void GenerateCubes()
    {
        foreach (var sphere in spheresList)
        {
            float stepSize = (2 * sphere.radius) / gridResolution;

            for (int x = 0; x < gridResolution; x++)
            {
                for (int y = 0; y < gridResolution; y++)
                {
                    for (int z = 0; z < gridResolution; z++)
                    {
                        Vector3 cubeCenter = new Vector3(
                            sphere.center.x - sphere.radius + x * stepSize,
                            sphere.center.y - sphere.radius + y * stepSize,
                            sphere.center.z - sphere.radius + z * stepSize
                        );

                        if (ShouldGenerateCube(cubeCenter))
                        {
                            cubeDictionary[cubeCenter] = new CubeData(CreateCube(cubeCenter, stepSize), 255);
                        }
                    }
                }
            }
        }
    }

    // V�rifie si le cube doit �tre g�n�r� en fonction des options
    private bool ShouldGenerateCube(Vector3 cubeCenter)
    {
        if (union && IsCubeInSphere(cubeCenter)) return true;
        if (intersect && IsCubeInAllSpheres(cubeCenter)) return true;
        if (difference && IsCubeOnlyInOneSphere(cubeCenter)) return true;
        return false;
    }

    // V�rifie si un cube est dans toutes les sph�res (intersection)
    bool IsCubeInAllSpheres(Vector3 cubeCenter)
    {
        foreach (var sphere in spheresList)
        {
            if (Vector3.Distance(cubeCenter, sphere.center) >= sphere.radius)
            {
                return false;
            }
        }
        return true;
    }

    // V�rifie si un cube est seulement dans une seule sph�re (diff�rence)
    bool IsCubeOnlyInOneSphere(Vector3 cubeCenter)
    {
        int count = 0;

        foreach (var sphere in spheresList)
        {
            if (Vector3.Distance(cubeCenter, sphere.center) < sphere.radius)
            {
                count++;
            }
        }
        return count == 1;
    }

    // V�rifie si un cube est dans au moins une sph�re (union)
    bool IsCubeInSphere(Vector3 cubeCenter)
    {
        foreach (var sphere in spheresList)
        {
            if (Vector3.Distance(cubeCenter, sphere.center) < sphere.radius)
            {
                return true;
            }
        }
        return false;
    }

    // Cr�e un cube � une position donn�e
    GameObject CreateCube(Vector3 position, float size)
    {
        GameObject cube = cubePrefab != null ? Instantiate(cubePrefab, position, Quaternion.identity) : GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * size;

        return cube;
    }

    // Mise � jour de la visibilit� en fonction du seuil
    void UpdateCubesVisibility()
    {
        foreach (var entry in cubeDictionary.Values)
        {
            var col = entry.cube.GetComponent<Renderer>().material.color;
            entry.cube.GetComponent<Renderer>().material.color = new Color(col[0], col[1], col[2], entry.potential/255f);
            entry.cube.SetActive(entry.potential > Threshold);
        }
    }

    // Mettre � jour le potentiel des cubes en fonction de la distance avec l'outil
    void UpdatePotentialWithTool()
    {
        Vector3 toolPosition = toolSphere.transform.position;
        float toolRadius = toolSphere.transform.localScale.x / 2;

        foreach (var entry in cubeDictionary)
        {
            Vector3 cubePos = entry.Key;
            CubeData cubeData = entry.Value;

            float distance = Vector3.Distance(cubePos, toolPosition);
            if (distance < toolRadius)
            {
                // Ajouter ou soustraire du potentiel en fonction de l'outil
                cubeData.potential += (toolSphere.GetComponent<toolControl>().erasing ? -toolInfluence : toolInfluence);
                cubeData.potential = Mathf.Clamp(cubeData.potential, 0f, 255f);
            }
        }
    }
}