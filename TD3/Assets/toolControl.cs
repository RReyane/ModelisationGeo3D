using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolControl : MonoBehaviour
{
    public float moveSpeed = 5f;     // Vitesse de d�placement de la sph�re
    public float scrollSpeed = 2f;   // Vitesse de changement de taille avec la molette
    public bool erasing = false;     // Indique si la sph�re est en mode effacement

    void Update()
    {
        // D�placement avant,arriere,gauche,droite avec les touches du clavier
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveAvantArriere = Input.GetAxis("Vertical");
        float moveVertical = 0;

        // DEplacement haut et bas avec les touches du clavier
        if (Input.GetKey(KeyCode.Space))
        {
            moveVertical += 1;
        }
        // Changer le mode de la sph�re avec la touche espace
        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveVertical -= 1;
        }

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, moveAvantArriere) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Changement de taille de la sph�re avec la molette de la souris
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.localScale += Vector3.one * scroll * scrollSpeed * Time.deltaTime;

        // Emp�cher la sph�re d'avoir une �chelle n�gative
        if (transform.localScale.x < 0.1f)
        {
            transform.localScale = Vector3.one * 0.1f;
        }

        // Changer le mode de la sph�re avec la touche espace
        if (Input.GetKeyDown(KeyCode.E))
        {
            erasing = !erasing;
            if (erasing)
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }
}
