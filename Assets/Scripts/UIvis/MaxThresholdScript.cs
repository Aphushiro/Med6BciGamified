using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxThresholdScript : MonoBehaviour
{
    Slider MaxSlider;
    public Slider absoluteSlider;

    private void Start()
    {
        MaxSlider = GetComponent<Slider>();
    }

    public void SetSliderMax (float MaxVal)
    {

        if (absoluteSlider.value > MaxSlider.value)
        {
            MaxSlider.value = absoluteSlider.value;
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            MaxSlider.value = 0;
        }
    }
}