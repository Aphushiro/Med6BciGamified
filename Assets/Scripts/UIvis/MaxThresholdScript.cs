using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxThresholdScript : MonoBehaviour
{
    Slider MaxSlider;
    public Slider absoluteSlider;
    
    public float maxHealth = 100;

    public float currentHealth = 100;
    private float _damageAmount;
    
    private Coroutine _myCoroutine;
    private bool _isCoroutineRunning = false;

    private void Start()
    {
        MaxSlider = GetComponent<Slider>();
        
    }

    public void SetSliderMax (float MaxVal)
    {

        if (absoluteSlider.value > MaxSlider.value)
        {
            MaxSlider.value = absoluteSlider.value;
            _damageAmount = MaxSlider.value;
        }
        
    }

    public void BCI_Action()
    {
        
        if (_isCoroutineRunning == false)
        {
            _myCoroutine = StartCoroutine(MyCoroutine());
        }
        
    }

    IEnumerator MyCoroutine()
    {
        _isCoroutineRunning = true;
        Debug.Log("Start New Coroutine!");
        // Wait for 6 seconds
        yield return new WaitForSeconds(6f);

        // Call your method here
        TakeDamage();
        MaxSlider.value = 0;

        // Coroutine ends here
    }

    
    public void TakeDamage()
    {
        currentHealth -= _damageAmount;
        Debug.Log("Health:"+currentHealth);
        
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            Debug.Log("Try: Destroy Wood");
        }
        
    }
    
    /*
    private void Update()
    {
        
        //Skal nok ændres til coroutines.
        //Fra input start og nogle sekunder frem, skal max værdi gemmes og nulstilles
        if (Input.GetKeyDown(KeyCode.V))
        {
            //MaxSlider.value = 0;
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            TakeDamage();
        }
    }
    
    */
    
    
    
}