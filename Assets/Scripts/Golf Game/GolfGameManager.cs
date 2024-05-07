using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random=UnityEngine.Random;
using System.Diagnostics;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class GolfGameManager : MonoBehaviour
{
    [Header("References")]
     LoggingBehaviour loggingBehaviour;
    
    public static GolfGameManager Instance;
    [SerializeField] private RotateGolfClub _club;
    [SerializeField] private float clubWindupLerpTime = 0.25f;
    [SerializeField] private float clubChopLerpTime = 0.05f;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private TextMeshProUGUI distanceNumberText;
    private GolfBallBehaviour _gbb;

    [Header("BCI Meter")]
    public float BCI_InputMinimum = 0.4f;
    
    public Slider MaxSlider;
    public Slider absoluteSlider;

    [Header("GameObject")]
    public GameObject golfBall;
    private GameObject ballCopy;
    public Transform ParentGameobject;
    
    public Image healthBarImage;
    public Image HealthBarImageRed;
    public Image ClockImage;
    
    [FormerlySerializedAs("maxHealth")] public float maxDistance = 100f;
    [FormerlySerializedAs("currentHealth")] public float currentDistance = 100f;
    
    private float _swingHitStrength;
    public float fillSpeed = 1f;
    
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip[] arraySounds;
    private int arrayMax;
    private int soundToPlay;

    [Header("Game Manager")]
    public bool isGameRunning = false;
    public bool hasGameRun = false;
    [SerializeField] private int amountOfHoles = 2;
    private float ballsPlayed = 0;
    private float lastBallPlayed = 0;
    public int activePeriodDuration = 6;
    public int breakPeriodDuration = 8;
   
    //[Header("Coroutine")]
    private Coroutine _activeTimeCoroutine;
    public bool _isRestingPeriod = false;
    private bool _isBallHit = false;
    private bool _isBallFlying;
    private Coroutine restTextCoroutine;
    private bool miDetected = false;
    
     //////////////////////////////////////////////////////////////////////////////////////////////////////
     
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
        arrayMax = arraySounds.Length;
        
        announcementText = GameObject.Find("AnnouncementText").GetComponent<TextMeshProUGUI>();
        distanceNumberText = GameObject.Find("DistanceNumberText").GetComponent<TextMeshProUGUI>();
        distanceNumberText.text = $"{currentDistance:F2} m";
        
        loggingBehaviour = FindObjectOfType<LoggingBehaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && isGameRunning == false && hasGameRun == false)
        {
            StartCoroutine(RunGame(amountOfHoles));
           loggingBehaviour.LogEvent("GameStarted"); 
        }  
        
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private IEnumerator RunGame(int totalBallsToPlay)
    {
        isGameRunning = true;
        
        ballCopy = Instantiate(golfBall, ParentGameobject.transform);
        _gbb = ballCopy.GetComponent<GolfBallBehaviour>();
        
        yield return StartCoroutine(StartBreakTime(breakPeriodDuration+2));

        while (ballsPlayed < totalBallsToPlay)
        {
            yield return StartCoroutine(StartActiveTime());
            if (ballsPlayed == totalBallsToPlay)
            {
                break;
                Debug.Log("Ending Game. Skipping last break");
            }
                
            yield return StartCoroutine(StartBreakTime(breakPeriodDuration));
        }
        
        isGameRunning = false;
        hasGameRun = true;
        
        yield return new WaitForSeconds(1f);
        announcementText.text = "You've putted all the balls!";
        yield return new WaitForSeconds(2f);
        announcementText.text = "The game is now over. Thank you for playing!!";
        StopAllCoroutines();
    }
    
    public float GetRotationFraction(float value)
    {
        //var returnValue = (value - BCI_InputMinimum) / (1 - BCI_InputMinimum);
        //return returnValue < 0 ? 0 : returnValue;
        return value;
    }
    
    public void SetSliderMax()
    {
        if (absoluteSlider.value > MaxSlider.value && !_isRestingPeriod && isGameRunning && !_isBallFlying)
        {
            MaxSlider.value = absoluteSlider.value;
            audioSource.volume = MaxSlider.value;
            
            _club.RotateClub(clubWindupLerpTime, GetRotationFraction(MaxSlider.value));
        }
        
        loggingBehaviour.LogGolfSample("Sample", absoluteSlider.value, MaxSlider.value, _isBallHit);
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
        miDetected = false;
        
        ClockImage.fillAmount = 1; //Resets clock fill after attack
        StartCoroutine(LerpAnimateClockActive(0f, activePeriodDuration));
        SetActivePeriodText();
        
        yield return new WaitForSeconds(activePeriodDuration - 0.5f);
        
        yield return new WaitForSeconds(0.5f);
        
        _club.RotateClub(clubChopLerpTime, 0f,true);
        
        //Debug.Log($"Maxslider: {MaxSlider.value}. BCI threshold: {BCI_InputMinimum}");
        
        if (miDetected)
        {
            yield return new WaitForSeconds(clubChopLerpTime/2);
            announcementText.text = "";
            _swingHitStrength = MaxSlider.value;
            ClockImage.color = Color.white;
            ClockImage.fillAmount = 1;
            HitBall();
        }
        else
        {           
            Debug.Log("MI Not Detected");
            _isBallHit = false;
            MaxSlider.value =  Math.Max(MaxSlider.value,0.1f);
            _club.RotateClub(clubWindupLerpTime, GetRotationFraction(MaxSlider.value));
        }
        
        miDetected = false;
    }

    private IEnumerator TrackBallAfterHit()
    {
        var distanceAtHit = currentDistance;
        
        while (true)
        {
            var lastBallPosX = ballCopy.transform.position.x;
            yield return new WaitForSeconds(0.05f);
            
            //Debug.Log("Waiting for ball to stop!");
            
            var currenBallPosX = ballCopy.transform.position.x;
            
            currentDistance = Math.Max(distanceAtHit - (float)Math.Round(currenBallPosX / 100, 2) , 0f);
            if (currentDistance == 0)
            {
                distanceNumberText.color = Color.green;
                distanceNumberText.text = $"{currentDistance:F2} m";
                _isBallHit = false;
                break;
            }
            distanceNumberText.text = $"{currentDistance:F2} m";
            
            if (lastBallPosX < currenBallPosX + 0.01f &&
                lastBallPosX > currenBallPosX - 0.01f)
            {
                //debug.Log("Ball has stopped!");
                break;
            }
        }
        
        if (currentDistance <= 0)
        {
            DestroyAndReplaceBall(2f);
            currentDistance = maxDistance;
            ballsPlayed++;
        }
    }

    private void DestroyAndReplaceBall(float delay = 0)
    {
        StartCoroutine(DestroyAndReplaceBallCoroutine(delay));
    }

    private IEnumerator DestroyAndReplaceBallCoroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        Destroy(ballCopy);
        ballCopy = Instantiate(golfBall, ParentGameobject.transform);
        _gbb = ballCopy.GetComponent<GolfBallBehaviour>();
    }

    public void HitBall()
    {
        _isBallHit = true;
        
        _gbb.HitBall(_swingHitStrength);
        
        soundToPlay = Random.Range(0, arrayMax);
        audioSource.clip = arraySounds[soundToPlay];
        audioSource.Play();
    }

    public void LogHitBall()
    {
        if (_isRestingPeriod) return;
        miDetected = true;
        loggingBehaviour.LogOnMiGolf("SuccessfulHit", absoluteSlider.value, MaxSlider.value, _isBallHit);
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
        loggingBehaviour.LogGolfResting("Resting", absoluteSlider.value, MaxSlider.value, currentDistance, maxDistance-currentDistance, maxDistance, _isBallHit);
        
        _isRestingPeriod = true;
        StartCoroutine(LerpAnimateClockBreak(1f, seconds));

        if (seconds == breakPeriodDuration + 2)
        {
            SetRestingPeriodText("Intro");
        }
        else if (_isBallHit)
        {
            StartCoroutine(LerpDistanceLerpOverTime(_swingHitStrength, seconds));
            SetRestingPeriodText("Hit");
        }
        else if (!_isBallHit)
        {
            SetRestingPeriodText("Miss");
        }
        else
        {
            SetRestingPeriodText();
        }
        
        yield return new WaitForSeconds(seconds);

        MaxSlider.value = 0;
        _isRestingPeriod = false;
    }

    IEnumerator LerpDistanceLerpOverTime(float hitStrength, float breakDuration)
    {
        float duration = 5f; // Duration of the lerp
        float startValue = 0f; // Start value
        float endValue = 25f * hitStrength; // End value is a percentage of 25 based on hitStrength
        float startDistance = currentDistance;

        float startTime = Time.time; // The time when the coroutine started

        while (Time.time < startTime + duration)
        {
            // Calculate the elapsed time and the interpolation factor
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;

            // Interpolate between the start and end values based on the interpolation factor
            float currentValue = Mathf.Lerp(startValue, endValue, t);
            currentDistance = startDistance - currentValue;
            
            if (currentDistance < 0)
            {
                currentDistance = 0;
                distanceNumberText.color = Color.green;
                distanceNumberText.text = $"{currentDistance:F2} m";
                _isBallHit = false;
                ballsPlayed++;
                currentDistance = maxDistance;
                lastBallPlayed = ballsPlayed;
                SetRestingPeriodText("LogChopped", breakDuration - elapsedTime);
                break;
            }
            distanceNumberText.text = $"{currentDistance:F2} m";
            // Use currentValue here...

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is exactly as intended
        // Use endValue here...
    }
    
    private void SetActivePeriodText()
    {
        Debug.Log("SetActivePeriodText");
        announcementText.color = Color.green;
        announcementText.text = PhraseManager.Instance.GetActivePhrase();
    }

    private void SetRestingPeriodText(string format = "", float timeLeft = 0f)
    {
        if (restTextCoroutine != null)
            StopCoroutine(restTextCoroutine);
        restTextCoroutine = StartCoroutine(SetBreakText(format, timeLeft));
    }

    private IEnumerator SetBreakText(string format = "", float timeLeft = 0f)
    {
        switch (format)
        {
            case "Intro":
                //debug.Log($"Intro. Format: {format}");
                announcementText.text = "The game is starting shortly.";
                yield return new WaitForSeconds(breakPeriodDuration);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                yield return new WaitForSeconds(2f);
                break;
            case "LogChopped":
                //debug.Log($"LogChopped. Format: {format}");
                announcementText.color = Color.white;
                announcementText.text = "";
                announcementText.text = $"Well done!\n{amountOfHoles-ballsPlayed} ball{(amountOfHoles-ballsPlayed != 1 ? "s" : "")} left!";
                yield return new WaitForSeconds(timeLeft-2);
                announcementText.color = Color.yellow;
                announcementText.text = ballsPlayed == amountOfHoles ? "" : "Get ready!!";
                distanceNumberText.color = Color.white;
                distanceNumberText.text = $"{currentDistance:F2} m";
                _club.RotateClub(clubWindupLerpTime*2, 0f);
                yield return new WaitForSeconds(2);
                DestroyAndReplaceBall();
                announcementText.text = "";
                break;
            case "Hit":
                //debug.Log($"Hit. Format: {format}");
                announcementText.color = Color.white;
                announcementText.text = "";
                yield return new WaitForSeconds(1f);
                announcementText.text = PhraseManager.Instance.GetRestPhrase();
                yield return new WaitForSeconds(4f);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                DestroyAndReplaceBall();
                _club.RotateClub(clubWindupLerpTime*2, 0f);
                yield return new WaitForSeconds(2f);
                announcementText.text = "";
                break;
            case "Miss":
                //debug.Log($"Miss. Format: {format}");
                announcementText.color = Color.red;
                announcementText.text = "";
                yield return new WaitForSeconds(1f);
                announcementText.text = "You missed! Try again.";
                yield return new WaitForSeconds(4f);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                _club.RotateClub(clubWindupLerpTime*2, 0f);
                yield return new WaitForSeconds(2f);
                announcementText.text = "";
                break;
            default:
                //debug.Log($"Default Text. Format: {format}");
                announcementText.color = Color.white;
                announcementText.text = "";
                yield return new WaitForSeconds(1f);
                announcementText.text = PhraseManager.Instance.GetRestPhrase();
                yield return new WaitForSeconds(4f);
                announcementText.color = Color.yellow;
                announcementText.text = "Get ready!!";
                _club.RotateClub(clubWindupLerpTime*2, 0f);
                yield return new WaitForSeconds(2f);
                announcementText.text = "";
                break;
        }
    }
}