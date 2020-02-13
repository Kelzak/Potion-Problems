using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camShake : MonoBehaviour
{
    public Camera gameCam;

    Vector3 originalPos;

    public float shakeAmount;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = gameCam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameCam.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount * Time.deltaTime;



    }
}
