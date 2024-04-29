using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxThresholdScript : MonoBehaviour
{
    public static MaxThresholdScript Instance;
    
    [HideInInspector]
    public Slider MaxSlider;
    public Slider absoluteSlider;
    
    public float maxHealth = 100;

    public float currentHealth = 100;
    private float _damageAmount;
    
    private Coroutine _myCoroutine;
    private bool _isCoroutineRunning = false;

   /* private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }
    */

    private void Start()
    {
        MaxSlider = GetComponent<Slider>();
        
    }

    public void SetSliderMax()
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
        
        
        yield return new WaitForSeconds(6f);

     
        TakeDamage();
        MaxSlider.value = 0;

        _isCoroutineRunning = false;
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