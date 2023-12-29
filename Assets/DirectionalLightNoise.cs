using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightNoise : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 10;
    
    private void Update()
    {
        transform.RotateAround(transform.position,Vector3.forward, Time.deltaTime * rotationSpeed);    
    }
}
