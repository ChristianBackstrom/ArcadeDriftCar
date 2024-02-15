using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private float minVelocity = .4f;
    [SerializeField] private float maxRotation = 30f;
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private float distanceFromTarget = 10;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = targetRigidbody.velocity;
        velocity.y = 0;
        
        Vector3 left = targetRigidbody.transform.forward;
        Vector3 forward = targetRigidbody.transform.right;

        velocity = velocity.magnitude < minVelocity ? forward : velocity;
        
        float dotProduct = 1 - Vector3.Dot(forward, velocity.normalized);
        bool driftingLeft = 0 < Vector3.Dot(left, velocity.normalized);

        Vector3 position = new Vector3(-(1 + dotProduct), 0,
            (driftingLeft ? -dotProduct : dotProduct));
        
        position.Normalize();
        position *= 10;
        position.y = transform.localPosition.y;
        
        transform.localPosition = position;

        Quaternion rotation = Quaternion.identity;

        rotation = Quaternion.AngleAxis(90 + (driftingLeft ? -dotProduct : dotProduct) * maxRotation, Vector3
            .up);

        transform.localRotation = rotation;
    }
}
