using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;
using System;

public class ClockMng : MonoBehaviour
{
    [Serializable]
    public class OnRestingEvent : UnityEvent<int, float> { }
    private LoggingBehaviour logCaller;

    public OnRestingEvent OnResting;
    public Image[] pieImg = new Image[3];
    public Image[] feedbackImg = new Image[3];

    public RectTransform handParent;

    float handSpeed = 24f;

    bool isResting = false;

    int lastState = 0;
    int currentlyOn = 0;
    [SerializeField]
    float[] clockValsMax = new float[3] { 0f, 0f, 0f };
    float curConfidence = -1f;

    bool gameIsStarted = false;

    private void Start()
    {
        logCaller = FindObjectOfType<LoggingBehaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameIsStarted == false)
            {
                logCaller.LogEvent("GameStarted");
            }

            gameIsStarted = true;
        }

        if (lastState != currentlyOn)
        {
            // Stuff we want to happen when clockState changes
            
            
            // Log last sample of state. Not needed if we log every sample anyways
            //logCaller.LogStateFinalSample("Sample", lastState, clockValsMax[lastState], clockValsMax);
            lastState = currentlyOn;
        }

        // Things to update while not resting
        if (isResting) { return; }
    }

    private void FixedUpdate()
    {
        if (gameIsStarted == false) { return; }
        float rotAmount = - (Time.deltaTime * (360f / handSpeed));
        handParent.Rotate(Vector3.forward, rotAmount);

        float zRot = handParent.rotation.eulerAngles.z;
        //Debug.Log(zRot);
        // Reset colors
        for (int i = 0; i < pieImg.Length; i++)
        {
            pieImg[i].color = new Color(1, 1, 1, 0.5f);
        }


        if (zRot < 45 || zRot > 315f) // Top (0)
        {
            if (isResting) { return; }
            currentlyOn = 0;
            isResting = true;
            StartCoroutine(WaitForRest(Game2Mng.Instance.restTime));

            return;
        } else
        {
            handSpeed = 24f; // 6 * 4 = 24
        }

        // Set color of choice
        if (zRot < 315f && zRot > 225f) // Right (1)
        {
            pieImg[0].color = Color.white;
            currentlyOn = 0;
        } else if (zRot < 225f && zRot > 135f) // Bottom (2)
        {
            pieImg[1].color = Color.white;
            currentlyOn = 1;
        } else // Left (3)
        {
            pieImg[2].color = Color.white;
            currentlyOn = 2;
        }
    }

    IEnumerator WaitForRest (float time)
    {
        handSpeed = time * 4; // 8 * 4 = 32
        int highState = GetClockState();
        OnResting.Invoke(highState, clockValsMax[highState]);
        logCaller.LogClockResting("Resting", curConfidence, clockValsMax);

        ResetMaxVals();
        yield return new WaitForSeconds(time + 0.1f);
        isResting = false;
    }

    public void ResetMaxVals()
    {
        clockValsMax = new float[3] { 0, 0, 0 };

        foreach (Image img in feedbackImg)
        {
            img.enabled = false;
        }
    }

    public void UpdateClockMax (float newVal)
    {
        curConfidence = newVal;

        // Runs in OnBCI event
        if (newVal > clockValsMax[currentlyOn])
        {
            if (isResting || gameIsStarted == false) 
            { ResetMaxVals(); } 
            else
            {
                clockValsMax[currentlyOn] = newVal;
                SetClockFeedback();
            }
        }
        logCaller.LogClockSample("Sample", newVal, currentlyOn, clockValsMax[currentlyOn], clockValsMax);
    }

    public int GetClockState ()
    {
        int higherState = 0;
        float highVal = 0f;
        for (int i = 0; i < clockValsMax.Length; i++)
        {
            if (clockValsMax[i] > highVal)
            {
                highVal = clockValsMax[i];
                higherState = i;
            }
        }

        return higherState;
    }

    void SetClockFeedback ()
    {
        if (clockValsMax == null || clockValsMax.Length == 0)
        {
            // Handle empty or null array case
            return;
        }

        int maxIndex = 0;
        float maxValue = clockValsMax[0];

        for (int i = 1; i < clockValsMax.Length; i++)
        {
            if (clockValsMax[i] > maxValue)
            {
                maxValue = clockValsMax[i];
                maxIndex = i;
            }
        }
        foreach (Image img in feedbackImg)
        {
            img.enabled = false;
        }

        feedbackImg[maxIndex].enabled = true;
    }
}
