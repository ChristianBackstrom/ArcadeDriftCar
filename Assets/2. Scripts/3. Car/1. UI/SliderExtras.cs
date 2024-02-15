using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderExtras : MonoBehaviour
{
    [SerializeField] private UnityEvent start;
    [SerializeField] private UnityEvent update;

    private void Start()
    {
        start.Invoke();
    }

    private void Update()
    {
        update.Invoke();
    }
}
