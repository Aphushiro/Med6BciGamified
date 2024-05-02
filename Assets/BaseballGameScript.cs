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
    
    public float forceMagnitude = 10f; // Adjust the force magnitude as needed
    public Rigidbody2D rb;
    
   
    
    public void SetSliderMax()
    {
        if (absoluteSlider.value > MaxSlider.value)
        {
            MaxSlider.value = absoluteSlider.value;
            
        }
    }
    
    public void BCI_Action()
    {
        if (_isCoroutineRunning == false)
        {
          //  _myCoroutine = StartCoroutine(MyCoroutine());
        }
        FireBall();
    }

    public void FireBall()
    {
        // Calculate the direction of the force
        Vector2 forceDirection = new Vector2(1, 1).normalized; // 45-degree angle

        // Apply the force to the ball
        rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
    }

    

    

    
    
}
