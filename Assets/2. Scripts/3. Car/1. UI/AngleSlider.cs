using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AngleSlider : MonoBehaviour
{
    [SerializeField] private Image leftRect, rightRect;
    [SerializeField] private DriftCar driftCar;

    private void Update()
    {
        Vector3 velocity = driftCar.Rigidbody.velocity;
        if (velocity.magnitude < 1)
        {
            leftRect.fillAmount = 0;
            rightRect.fillAmount = 0;
            return;
        }
        
        Vector3 left = driftCar.transform.forward;
        Vector3 forward = driftCar.transform.right;
        velocity.y = 0;

        float dotProduct = 1 - Vector3.Dot(forward, velocity.normalized);
        bool driftingLeft = 0 < Vector3.Dot(left, velocity.normalized);

        float leftSliderValue = driftingLeft ? dotProduct : 0;
        float rightSliderValue = driftingLeft ? 0 : dotProduct;

        leftRect.fillAmount = leftSliderValue;
        rightRect.fillAmount = rightSliderValue;
    }
}
