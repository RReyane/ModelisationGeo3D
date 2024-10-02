using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class Charge : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public Material material;
    public string nomFichier = "buddha";

    void ReadOFF(ref List<Vector3> verticesList, ref List<int> trianglesList)
    {
        using (StreamReader File = new StreamReader("..\\off\\" + nomFichier + ".off"))
        {
            //verifie qu'on as bien un fichier .off
            string ligne = File.ReadLine();
            if (ligne != "OFF")
            {
                throw new System.Exception("Pas un fichier .OFF");
            }

            //lire le nombre de vertices et triangles
            ligne = File.ReadLine();
            string[] nbDimensions = ligne.Split(' ');
            int nbVertices = int.Parse(nbDimensions[0]);
            int nbTriangles = int.Parse(nbDimensions[1]);

            //remplir le tableau de vertices
            for (int i = 0; i < nbVertices; i++)
            {
                ligne = File.ReadLine();
                string[] coords = ligne.Split(' ');
                float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                float y = float.Parse(coords[1], CultureInfo.InvariantCulture);
                float z = float.Parse(coords[2], CultureInfo.InvariantCulture);
                verticesList.Add(new Vector3(x, y, z));
            }

            //remplir le tableau de triangles
            for (int i = 0; i < nbTriangles; i++)
            {
                ligne = File.ReadLine();
                string[] indice = ligne.Split(' ');
                trianglesList.Add(int.Parse(indice[1]));
                trianglesList.Add(int.Parse(indice[2]));
                trianglesList.Add(int.Parse(indice[3]));
            }
        }
    }

    void Trace(ref List<Vector3> verticesList, ref List<int> trianglesList) {
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.normals = calculNormales(ref verticesList, ref trianglesList);
    }

    Vector3 CalculCentre(ref List<Vector3> verticesList) {
        
        Vector3 centre = new Vector3();

        foreach (var vertice in verticesList)
        {
            centre += vertice;
        }

        return centre/ verticesList.Count;
    }

    void Centrer(ref List<Vector3> verticesList){
        var temp = CalculCentre(ref verticesList);
        for (int i = 0; i < verticesList.Count; i++) {
            verticesList[i] += (Vector3.zero - temp);
        }
    }

    void normaliseTaille(ref List<Vector3> verticesList){
        float max = 0;
        foreach (var vertice in verticesList)
        {
            max = Mathf.Max(max, Mathf.Abs(vertice.x), Mathf.Abs(vertice.y), Mathf.Abs(vertice.z));
        }

        // Diviser toutes les coordonnées par cette valeur maximale
        for (int i = 0; i < verticesList.Count; i++)
        {
            verticesList[i] /= max;
        }
    }

    Vector3[] calculNormales(ref List<Vector3> verticesList, ref List<int> trianglesList)
    {
        Vector3[] normales = new Vector3[verticesList.Count];

        for (int i = 0; i < normales.Length; i++)
        {
            normales[i] = Vector3.zero;
        }

        //produit vectoriel de 2 vecteur du triangles

        // Pour chaque triangle
        for (int i = 0; i < trianglesList.Count; i += 3)
        {
            // Indices des sommets du triangle
            int index0 = trianglesList[i];
            int index1 = trianglesList[i + 1];
            int index2 = trianglesList[i + 2];

            // Sommets du triangle
            Vector3 v0 = verticesList[index0];
            Vector3 v1 = verticesList[index1];
            Vector3 v2 = verticesList[index2];

            // Calculer les deux vecteurs de l'arête
            Vector3 edge1 = v1 + (Vector3.zero - v0);
            Vector3 edge2 = v2 + (Vector3.zero - v0);

            // Calculer la normale du triangle via le produit vectoriel
            Vector3 normale = Vector3.Cross(edge2, edge1).normalized;

            // Ajouter la normale aux trois sommets du triangle
            normales[index0] += normale;
            normales[index1] += normale;
            normales[index2] += normale;
        }

        return normales;
    }

    void Export(string nomFichier, ref List<Vector3> verticesList, ref List<int> trianglesList, Vector3[] normalesList)
    {
        using (StreamWriter sw = new StreamWriter("..\\export\\" + nomFichier + ".obj"))
        {
            sw.WriteLine("#Export de fichier .off passé dans Unity\n#\n");
            sw.WriteLine("o object\n");

            //vertices
            for (int i = 0; i < verticesList.Count; i++)
            { sw.WriteLine("v " + verticesList[i].x.ToString(CultureInfo.InvariantCulture) + " " + verticesList[i].y.ToString(CultureInfo.InvariantCulture) + " " + verticesList[i].z.ToString(CultureInfo.InvariantCulture)); }

            sw.WriteLine("\n");

            //texture
            for (int i = 0; i < verticesList.Count; i++)
            { sw.WriteLine("vt 0.0 0.0"); }

            sw.WriteLine("\n");

            //normale
            for (int i = 0; i < normalesList.Length; i++)
            { sw.WriteLine("vn " + normalesList[i].x.ToString(CultureInfo.InvariantCulture) + " " + normalesList[i].y.ToString(CultureInfo.InvariantCulture) + " " + normalesList[i].z.ToString(CultureInfo.InvariantCulture)); }

            sw.WriteLine("\n");

            //faces
            for (int i = 0; i < trianglesList.Count; i += 3)
            { sw.WriteLine("f "+(trianglesList[i]+1)   + "/" + (trianglesList[i]+1)   + "/" + (trianglesList[i]+1) 
                           +" "+(trianglesList[i+1]+1) + "/" + (trianglesList[i+1]+1) + "/" + (trianglesList[i+1]+1) 
                           +" "+(trianglesList[i+2]+1) + "/" + (trianglesList[i+2]+1) + "/" + (trianglesList[i+2]+1)); }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = material;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        ReadOFF(ref vertices, ref triangles);

        Centrer(ref vertices);

        normaliseTaille(ref vertices);

        Trace(ref vertices, ref triangles);

        Export(nomFichier,ref vertices, ref triangles, mesh.normals);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
