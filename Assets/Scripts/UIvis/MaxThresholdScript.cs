using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxThresholdScript : MonoBehaviour
{
    public static MaxThresholdScript Instance;
    
    //BCI Meter
    public Slider MaxSlider;
    public Slider absoluteSlider;
    
    //GameObject
    public GameObject GameObject;
    public Image healthBarImage;
    public Image HealthBarImageRed;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    private float _damageAmount;
    public float fillSpeed = 1f;

    //Coroutine
    private Coroutine _myCoroutine;
    private bool _isCoroutineRunning = false;

    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //MaxSlider = GetComponent<Slider>();
        
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
        UpdateHealthBar();
        Debug.Log("Health:"+currentHealth);
        
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            
            Debug.Log("Try: Destroy Wood");
        }
        
    }
    
    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth;
        HealthBarImageRed.fillAmount = fillAmount;
        StartCoroutine(LerpFillAmount(fillAmount));
    }
    
    IEnumerator LerpFillAmount(float targetFillAmount)
    {
        float originalFillAmount = healthBarImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < fillSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (elapsedTime / fillSpeed), 3); // Exponential interpolation
            healthBarImage.fillAmount = Mathf.Lerp(originalFillAmount, targetFillAmount, t);
            yield return null;
        }

        healthBarImage.fillAmount = targetFillAmount;
        if (healthBarImage.fillAmount == 0)
        {
            Debug.Log("YoWhat");
            //yield return new WaitForSeconds(2f);
            HealthBarImageRed.fillAmount = 1;
            healthBarImage.fillAmount = 1;
            Debug.Log("The fuck");
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