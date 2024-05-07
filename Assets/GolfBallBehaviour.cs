using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallBehaviour : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float forceAmplification = 100f;
    [SerializeField] private float xFactor = 1;
    [SerializeField] private float yFactor = 3;
    
    private Coroutine exitCoroutine;

    private bool _isGrounded = false;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(OnCollision2DDelay());
    }

    public void HitBall(float forceFloat)
    {
        var totalForce = forceFloat * forceAmplification;
        var forceVector = new Vector3(totalForce * xFactor, totalForce * yFactor, 0f);
        _rb.AddForce(forceVector);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _isGrounded = true;
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if (exitCoroutine != null)
            StopCoroutine(exitCoroutine);
        exitCoroutine = StartCoroutine(ExitDelay(0.5f));
    }

    IEnumerator ExitDelay(float t)
    {
        yield return new WaitForSeconds(t);
        _isGrounded = false;
    }
    
    IEnumerator OnCollision2DDelay()
    {
        float duration; // Duration over which to change the angular drag
        float startAngularDrag; // Initial angular drag
        float targetAngularDrag; // Target angular drag
        float elapsed; // Time elapsed since the start of the interpolation
        bool previousGroundedState = _isGrounded; // The grounded state in the previous frame

        while (true)
        {
            startAngularDrag = _rb.angularDrag;
            targetAngularDrag = _isGrounded ? 10f : 0f;
            duration = _isGrounded ? 2f : 0.25f;
            elapsed = 0f;

            while (elapsed < duration)
            {
                // If the grounded state has changed since the last frame, start interpolating towards the new target value
                if (_isGrounded != previousGroundedState)
                {
                    startAngularDrag = _rb.angularDrag;
                    targetAngularDrag = _isGrounded ? 10f : 0f;
                    duration = _isGrounded ? 2f : 0.25f;
                    elapsed = 0f;
                }

                elapsed += Time.deltaTime;
                _rb.angularDrag = Mathf.Lerp(startAngularDrag, targetAngularDrag, elapsed / duration);
                previousGroundedState = _isGrounded;
                yield return null;
            }

            _rb.angularDrag = targetAngularDrag;
            yield return null;
        }
    }
}
