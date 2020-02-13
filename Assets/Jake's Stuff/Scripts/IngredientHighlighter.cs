using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHighlighter : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }        
    }
}
