using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GripController : MonoBehaviour
{
    [SerializeField] private PhysicMaterial rearWheel;
    [SerializeField] private PhysicMaterial frontWheel;

    public void RearWheelFriction(float value)
    {
        rearWheel.dynamicFriction = value;
        rearWheel.staticFriction = value;
    }
    
    public void FrontWheelFriction(float value)
    {
        frontWheel.dynamicFriction = value;
        frontWheel.staticFriction = value;
    }

    public void SetSliderRearWheel(Slider slider)
    {
        slider.value = rearWheel.dynamicFriction;
    }
    
    
    public void SetSliderFrontWheel(Slider slider)
    {
        slider.value = frontWheel.dynamicFriction;
    }
}
