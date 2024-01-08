using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class TransformNoise : MonoBehaviour
{
    [SerializeField]
    private float rotationAngularSpeed = 270;
    
    private Vector3 currentTargetDirection = Vector3.forward;

    [SerializeField]
    private float directionChoicePeriod = 2f;

    private float counter;
    
    void Update()
    {
        counter += Time.deltaTime;

        bool shouldChooseNewDirection = counter > directionChoicePeriod;
        if (shouldChooseNewDirection)
        {
            counter %= directionChoicePeriod;
            currentTargetDirection = Random.onUnitSphere;
        }

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float maxDegreesDelta = rotationAngularSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(currentTargetDirection, Vector3.up), maxDegreesDelta);
    }
}
