using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeScript : MonoBehaviour
{
    public Transform pivotPoint; // The point around which the axe will swing
    public float swingRange = 90f; // The maximum angle the axe can swing in degrees

    void Update()
    {
        
        float swingAmount = Mathf.Lerp(-swingRange, swingRange, MaxThresholdScript.Instance.MaxSlider.value
        ); // YourInputValue should be between 0 and 1
        transform.rotation = pivotPoint.rotation * Quaternion.Euler(0f, 0f, swingAmount);
        
    }
}
