using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random=UnityEngine.Random;

public class BaseballGameScript : MonoBehaviour
{
    public Rigidbody Ball;
    
    //BCI Meter
    public Slider MaxSlider;
    public Slider absoluteSlider;
    
    //Coroutine
    private Coroutine _myCoroutine;
    private bool _isCoroutineRunning = false;
    
    public float forceMultiplier = 10f; // You can adjust this value according to your requirements
    public float maxNoiseAngle = 20f; // Maximum angle for noise in degrees
    
    void Start()
    {
        
    }
    
    
    public void SetSliderMax()
    {
        if (absoluteSlider.value > MaxSlider.value)
        {
            MaxSlider.value = absoluteSlider.value;
            
        }
    }
    
    public void BCI_Action()
    {
        FireBall();
        if (_isCoroutineRunning == false)
        {
          //  _myCoroutine = StartCoroutine(MyCoroutine());
           
        }
    }
    
   

    

    

    void FireBall()
    {
        // Get the original direction the ball should be fired in
        Vector3 originalDirection = transform.forward;

        // Add noise to the direction
        Vector3 noisyDirection = Quaternion.Euler(Random.Range(-maxNoiseAngle, maxNoiseAngle), Random.Range(-maxNoiseAngle, maxNoiseAngle), 0) * originalDirection;

        // Apply force to the ball
        Ball.AddForce(noisyDirection * MaxSlider.value, ForceMode.Impulse);
    }
    
   
    
}
