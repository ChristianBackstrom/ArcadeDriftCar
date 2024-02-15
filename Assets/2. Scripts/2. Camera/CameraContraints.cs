using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContraints : MonoBehaviour
{
    [SerializeField] private bool lockX, lockY, lockZ;

    private Quaternion startRotation;
    private void Awake()
    {
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = transform.rotation;
        if (lockX) rotation.x = startRotation.x;
        if (lockY) rotation.y = startRotation.y;
        if (lockZ) rotation.z = startRotation.z;

        transform.rotation = rotation;
    }
}
