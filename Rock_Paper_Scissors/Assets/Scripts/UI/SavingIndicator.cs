using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingIndicator : MonoBehaviour
{
    [SerializeField] private Transform spinnerTransform;
    [SerializeField] private float maxRotationSpeed = 540f;
    [SerializeField] private float minRotationSpeed = 360f;
    private float rotationSpeed = 360f;


    void Update()
    {
        rotationSpeed = ((360f - spinnerTransform.rotation.eulerAngles.z) / 360) * (maxRotationSpeed-minRotationSpeed) + minRotationSpeed;
        spinnerTransform.Rotate(spinnerTransform.forward, rotationSpeed*Time.deltaTime);
    }
}
