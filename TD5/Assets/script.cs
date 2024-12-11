using System.Collections.Generic;
using UnityEngine;

public class ChaikinCurve : MonoBehaviour
{
    [SerializeField] public List<Vector3> controlPoints;  // Points de contrôle de la courbe
    [Range(0,5)]
    [SerializeField] private int subdivisions = 3;         // Nombre de subdivisions de la courbe

    private void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Count < 2)
            return;

        Gizmos.color = Color.red;

        // Obtenir la courbe subdivisée
        List<Vector3> subdividedCurve = ApplyChaikin(controlPoints, subdivisions);

        // Dessiner la courbe avec Gizmos.DrawLine
        for (int i = 0; i < subdividedCurve.Count - 1; i++)
        {
            Gizmos.DrawLine(subdividedCurve[i], subdividedCurve[i + 1]);
            //pour le dernier i faire avec [O]
        }

        //if (subdividedCurve.Count > 1)
        //{
        //    Gizmos.DrawLine(subdividedCurve[subdividedCurve.Count - 1], subdividedCurve[0]);
        //}
    }

    private List<Vector3> ApplyChaikin(List<Vector3> points, int iterations)
    {
        List<Vector3> subdividedPoints = new List<Vector3>(points);

        // Répéter l'algorithme pour le nombre de subdivisions souhaité
        for (int i = 0; i < iterations; i++)
        {
            subdividedPoints = ChaikinSubdivision(subdividedPoints);
        }

        return subdividedPoints;
    }

    private List<Vector3> ChaikinSubdivision(List<Vector3> points)
    {
        List<Vector3> newPoints = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];

            // Calcul des nouveaux points à 25 % et 75 % entre p0 et p1
            Vector3 q = Vector3.Lerp(p0, p1, 0.25f);
            Vector3 r = Vector3.Lerp(p0, p1, 0.75f);

            newPoints.Add(q);
            newPoints.Add(r);
        }

        return newPoints;
    }
}