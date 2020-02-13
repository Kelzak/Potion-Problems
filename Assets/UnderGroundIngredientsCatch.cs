using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGroundIngredientsCatch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("UnderGround"))
        {
            gameObject.transform.position = new Vector3(0, 2, -11);
        }
    }
}
