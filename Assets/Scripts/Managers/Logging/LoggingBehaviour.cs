using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Burst.CompilerServices;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class LoggingBehaviour : MonoBehaviour
{

    LoggingManager loggingManager;

    private void Start()
    {
        loggingManager = FindObjectOfType<LoggingManager>();
    }

    public void EndGame()
    {
        LogEvent("GameStopped");
        loggingManager.SaveLog("Game");
        loggingManager.SaveLog("Sample");
        loggingManager.SaveLog("Meta");
        loggingManager.ClearAllLogs();
    }

    void OnApplicationQuit()
    {
        EndGame();
    }

    public void LogEvent(string eventLabel)
    {
        Dictionary<string, object> gameLog = new Dictionary<string, object>() {
            {"Event", eventLabel},
        };
        loggingManager.Log("Game", gameLog);
    }

    public void LogClockResting(string eventLabel, float curConf, float[] maxConf)
    {
        string maxArr = "(";
        foreach (float t in maxConf)
        {
            maxArr += t.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture) + " ";
        }
        maxArr += ")";

        Dictionary<string, object> gameLog = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"MaxConfidenceArr", maxArr}
        };
        loggingManager.Log("Game", gameLog);
    }

    public void LogClockSample (string eventLabel, float curConf, int curState, float stateMax, float[] maxConf)
    {

        Dictionary<string, object> clockSample = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"ClockState", curState},
            {"StateMaxConf", stateMax},
            {"ColMax0", maxConf[0]},
            {"ColMax1", maxConf[1]},
            {"ColMax2", maxConf[2]}
        };

        loggingManager.Log("Sample", clockSample);
    }

    public void LogStateFinalSample (string eventLabel, float curConf, int curState, float stateMax, float[] maxConf)
    {
        string maxArr = "(";
        foreach (float t in maxConf)
        {
            maxArr += t.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture) + " ";
        }
        maxArr += ")";

        Dictionary<string, object> maxSample = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"ClockState", curState},
            {"StateMaxConf", stateMax},
            {"MaxConfidenceArr", maxArr}
        };
        loggingManager.Log("Sample", maxSample);

    }

    // Lumberjack logging
    public void LogLumberResting (string eventLabel, float curConf, float maxConf, float damage, float remainDam)
    {
        Dictionary<string, object> gameLog = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"MaxConfidence", maxConf},
            {"Damage", damage},
            {"DamRemaining", remainDam}
        };
        loggingManager.Log("Game", gameLog);
    }

    public void LogLumberSample(string eventLabel, float curConf, float maxConf)
    {
        Dictionary<string, object> logSample = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"ClockState", maxConf},
        };

        loggingManager.Log("Sample", logSample);
    }

    // Golf logging
    public void LogGolfResting(string eventLabel, float curConf, float maxConf, float distance, float remainDist, bool hit)
    {
        Dictionary<string, object> gameLog = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"MaxConfidence", maxConf},
            {"Distance", distance},
            {"DistRemaining", remainDist},
            {"BallWasHit", hit}
        };
        loggingManager.Log("Game", gameLog);
    }

    public void LogGolfSample (string eventLabel, float curConf, float maxConf, bool hit)
    {
        Dictionary<string, object> golfSample = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"MaxConfidence", maxConf},
            {"BallWasHit", hit},
        };

        loggingManager.Log("Sample", golfSample);
    }

    public void LogOnMiGolf (string eventLabel, float curConf, float maxConf, bool hit)
    {
        Dictionary<string, object> golfMi = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"BCIConfidence", curConf},
            {"MaxConfidence", maxConf},
            {"BallWasHit", hit},
        };

        loggingManager.Log("Game", golfMi);
    }
}
