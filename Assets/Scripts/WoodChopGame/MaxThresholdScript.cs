using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random=UnityEngine.Random;

public class MaxThresholdScript : MonoBehaviour
{
    [Header("References")]
    public static MaxThresholdScript Instance;
    public DestroyLogScript destroyLogScript;
    [SerializeField] private AxeScript _axe;
    [SerializeField] private float axeWindupLerpTime = 0.25f;
    [SerializeField] private float axeChopLerpTime = 0.05f;
    [SerializeField] private TextMeshProUGUI announcementText;

    [Header("BCI Meter")]
    public float BCI_InputMinimum = 0.4f;
    
    public Slider MaxSlider;
    public Slider absoluteSlider;

    [Header("GameObject")]
    public GameObject ChopLog;
    private GameObject logCopy;
    public Transform ParentGameobject;
    
    public Image healthBarImage;
    public Image HealthBarImageRed;
    public Image ClockImage;
    
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    
    private float _damageAmount;
    public float fillSpeed = 1f;
    
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip[] arraySounds;
    private int arrayMax;
    private int soundToPlay;

    [Header("Game Manager")]
    public bool isGameRunning = false;
    public bool hasGameRun = false;
    [SerializeField] private int amountOfLogs = 2;
    private float logsChopped = 0;
    private float lastLogsChopped = 0;
    public int activePeriodDuration = 6;
    public int breakPeriodDuration = 8;
   
    //[Header("Coroutine")]
    private Coroutine _activeTimeCoroutine;
    public bool _isRestingPeriod = false;
    
     //////////////////////////////////////////////////////////////////////////////////////////////////////
     
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
        logCopy = Instantiate(ChopLog, ParentGameobject.transform);
        arrayMax = arraySounds.Length;
        
        announcementText = GameObject.Find("AnnouncementText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && isGameRunning == false && hasGameRun == false)
            StartCoroutine(RunGame(amountOfLogs));
        
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private IEnumerator RunGame(int totalLogs)
    {
        isGameRunning = true;
        
        yield return StartCoroutine(StartBreakTime(breakPeriodDuration+2));

        while (logsChopped < totalLogs)
        {
            Debug.Log($"YOU'RE CHOPPING LOG NUMBER {logsChopped + 1}! AFTER THIS THERE ARE {totalLogs - logsChopped - 1} LOGS LEFT: ");
            yield return StartCoroutine(StartActiveTime());
            if (logsChopped == totalLogs)
                break;
            yield return StartCoroutine(StartBreakTime(breakPeriodDuration));
        }
        
        isGameRunning = false;
        hasGameRun = true;
        
        yield return new WaitForSeconds(1f);
        announcementText.text = "You've chopped all the logs!";
        yield return new WaitForSeconds(2f);
        announcementText.text = "The game is now over. Thank you for playing!!";
        StopAllCoroutines();
    }
    
    public float GetRotationFraction(float value)
    {
        var returnValue = (value - BCI_InputMinimum) / (1 - BCI_InputMinimum);
        return returnValue < 0 ? 0 : returnValue;
    }
    
    public void SetSliderMax()
    {
        if (absoluteSlider.value > MaxSlider.value && !_isRestingPeriod && isGameRunning)
        {
            MaxSlider.value = absoluteSlider.value;
            audioSource.volume = MaxSlider.value;
            
            _axe.RotateAxe(axeWindupLerpTime, GetRotationFraction(MaxSlider.value));
        }
    }

    public void BCI_Action()
    {
        if (_activeTimeCoroutine != null)
        {
            _activeTimeCoroutine = StartCoroutine(StartActiveTime());
           // StartCoroutine(WaitForTime(8));
        }
    }

    IEnumerator StartActiveTime()
    {
        ClockImage.fillAmount = 1; //Resets clock fill after attack
        StartCoroutine(LerpAnimateClockActive(0f, activePeriodDuration));
        SetActivePeriodText();
        
        yield return new WaitForSeconds(activePeriodDuration - 0.5f);

        if (MaxSlider.value < BCI_InputMinimum + 0.03f)
        {
            MaxSlider.value = BCI_InputMinimum + 0.03f;
            _axe.RotateAxe(axeWindupLerpTime, GetRotationFraction(MaxSlider.value));
        }
        
        yield return new WaitForSeconds(0.5f);
        
        _damageAmount = MaxSlider.value;
        
        MaxSlider.value = 0;  //Resets MaxValue of slider
        Debug.Log($"MaxSlider.value set to : {MaxSlider.value}");
        
        _axe.RotateAxe(axeChopLerpTime, 0f,true);
        
        yield return new WaitForSeconds(axeChopLerpTime);
        
        TakeDamage();
    }


    public void TakeDamage()
    {

        if (BCI_InputMinimum >= 0.4)
        {
            currentHealth -= _damageAmount- BCI_InputMinimum;
        }
        
        UpdateHealthBar();
        
        soundToPlay = Random.Range(0, arrayMax);
        audioSource.clip = arraySounds[soundToPlay];
        audioSource.Play();
        Debug.Log("Health:" + currentHealth);
        
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            Debug.Log("Try: Destroy Wood");
            Destroy(logCopy);
            logsChopped++;
            StartCoroutine(SpawnNewLogAfterDelay(breakPeriodDuration - axeChopLerpTime));
        }
    }
    
    private IEnumerator SpawnNewLogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay-2);
        logCopy = Instantiate(ChopLog, ParentGameobject.transform);
        HealthBarImageRed.fillAmount = 1;
        healthBarImage.fillAmount = 1;
    }

    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth;
        HealthBarImageRed.fillAmount = fillAmount;
        StartCoroutine(LerpAnimateHealthBar(fillAmount));
        
    }

    IEnumerator LerpAnimateHealthBar(float targetFillAmount)
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
    }

    IEnumerator LerpAnimateClockActive(float targetFillAmount, float duration)
    {        
        ClockImage.color = Color.green;
        ClockImage.fillClockwise = false;
        
        float startTime = Time.time;
        float startFillAmount = ClockImage.fillAmount;

        while (Time.time < startTime + duration)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration; // Calculate interpolation factor


            ClockImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, t);
            yield return null;
        }
        ClockImage.fillAmount = targetFillAmount; // Ensure final value is exactly as intended
    }
    
    IEnumerator LerpAnimateClockBreak(float targetFillAmount, float duration)
    {
        ClockImage.fillAmount = 0f;
        ClockImage.color = Color.white;
        ClockImage.fillClockwise = true;
        
        float startTime = Time.time;
        float startFillAmount = 1 - ClockImage.fillAmount; // Invert the start fill amount

        while (Time.time < startTime + duration)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration; // Calculate interpolation factor

            ClockImage.fillAmount = 1 - Mathf.Lerp(startFillAmount, 1 - targetFillAmount, t); // Invert the fill amount
            yield return null;
        }
        ClockImage.fillAmount = 1 - targetFillAmount; // Ensure final value is exactly as intended
    }
    
    IEnumerator StartBreakTime(float seconds)
    {
        _isRestingPeriod = true;
        StartCoroutine(LerpAnimateClockBreak(1f, seconds));

        if (seconds == breakPeriodDuration + 2)
        {
            SetRestingPeriodText("Intro");
        }
        else if (lastLogsChopped != logsChopped)
        {
            lastLogsChopped = logsChopped;
            SetRestingPeriodText("LogChopped");
        }
        else
        {
            SetRestingPeriodText();
        }
        
        yield return new WaitForSeconds(seconds);

        MaxSlider.value = 0;
        _isRestingPeriod = false;
    }

    private void SetActivePeriodText()
    {
        announcementText.color = Color.green;
        announcementText.text = PhraseManager.Instance.GetActivePhrase();
    }

    private void SetRestingPeriodText(string format = "")
    {
        StartCoroutine(SetBreakText(format));
    }

    private IEnumerator SetBreakText(string format = "")
    {
        switch (format)
        {
            case "Intro":
                announcementText.text = "The game is starting shortly.";
                yield return new WaitForSeconds(breakPeriodDuration);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                yield return new WaitForSeconds(2f);
                break;
            case "LogChopped":
                announcementText.color = Color.white;
                announcementText.text = "";
                yield return new WaitForSeconds(1f);
                announcementText.text = $"That's {logsChopped} of {amountOfLogs} logs. Keep going!";
                yield return new WaitForSeconds(4f);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                yield return new WaitForSeconds(2f);
                announcementText.text = "";
                break;
            default:
                announcementText.color = Color.white;
                announcementText.text = "";
                yield return new WaitForSeconds(1f);
                announcementText.text = PhraseManager.Instance.GetRestPhrase();
                yield return new WaitForSeconds(4f);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                yield return new WaitForSeconds(2f);
                announcementText.text = "";
                break;
        }
    }
}