using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbsoluteUiVis : MonoBehaviour
{
    Slider absoluteSlider;
    private void Start()
    {
        absoluteSlider = GetComponent<Slider>();
    }

    public void SetSliderVal (float val)
    {
        absoluteSlider.value = val;
    }
}
