using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AxeScript : MonoBehaviour
{
    public Transform pivotPoint; // The point around which the axe will swing
    public float swingRange = 90f; // The maximum angle the axe can swing in degrees
    private Coroutine _swingCoroutine;
    
    private bool _isSwingBlocked= false;

    private void Start()
    {
        transform.rotation = pivotPoint.rotation * Quaternion.Euler(0f, 0f, -90f); 
    }
    
    public void RotateAxe(float axeLerpTime, float axeLerpValue, bool blockSwing = false)
    {
        Debug.Log($"_isSwingBlocked: {_isSwingBlocked}, blockSwing: {blockSwing}");
        if (_isSwingBlocked)
            return;
        
        if (blockSwing)
            _isSwingBlocked = true;
        
        if (_swingCoroutine != null)
            StopCoroutine(_swingCoroutine);
        
        Debug.Log($"Starting Coroutine with axeLerpTime: {axeLerpTime} and axeLerpValue: {axeLerpValue}, blockSwing = {blockSwing}, _isSwingBlocked = {_isSwingBlocked}");
        _swingCoroutine = StartCoroutine(LerpRotationOverTime(axeLerpTime, axeLerpValue, blockSwing));
    }
    
    IEnumerator LerpRotationOverTime(float lerpTime, float lerpValue, bool blockSwing = false)
    {
        var localSwingRange = swingRange;
        
        Quaternion initialRotation = transform.rotation; // The initial rotation of the axe
        float swingAmount = Mathf.Lerp(-localSwingRange, localSwingRange, lerpValue); // YourInputValue should be between 0 and 1
        Quaternion finalRotation = pivotPoint.rotation * Quaternion.Euler(0f, 0f, swingAmount); // The final rotation of the axe

        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpTime; // Calculate interpolation factor

            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, t); // Lerp the rotation of the axe
            yield return null;
        }
        if (blockSwing)
        {
            _isSwingBlocked = false;
            Debug.Log("Setting _isSwingBlocked to false");
        }
        transform.rotation = finalRotation; // Ensure final rotation is exactly as intended
    }
}
