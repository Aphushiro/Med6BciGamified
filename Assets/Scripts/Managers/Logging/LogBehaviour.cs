using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMng;

public class LogBehaviour : MonoBehaviour
{
    /// 
    /// ------------- Every BCI event:
    /// ID
    /// Confidence
    /// 
    /// ------------- When user performs:
    /// ID
    /// Timestamp
    /// Framecount
    /// Confidence
    /// Classification threshold
    /// BCIThresholdBuffer
    /// Buffer size
    /// 

    LoggingManager loggingManager;

    private void Start()
    {
        loggingManager = FindObjectOfType<LoggingManager>();
    }
    /*
    private void LogEvent(string eventLabel)
    {
        Dictionary<string, object> gameLog = new Dictionary<string, object>() {
            {"Event", eventLabel},
            {"InputWindow", System.Enum.GetName(typeof(InputWindowState), inputWindow)},
            {"InputWindowOrder", inputIndex},
            {"InterTrialTimer", interTrialTimer},
            {"InputWindowTimer", inputWindowTimer},
            {"GameState", System.Enum.GetName(typeof(GameState), gameState)},
            {"CurrentFabAlarm", currentFabAlarm},
        };

        foreach (KeyValuePair<string, Mechanism> pair in mechanisms)
        {
            var m = pair.Value;
            gameLog[m.name + "TrialsLeft"] = m.trialsLeft;
            gameLog[m.name + "Rate"] = m.rate;
        }

        if (eventLabel == "GameDecision")
        {
            gameLog["TrialGoal"] = trialGoal;
            gameLog["TrialResult"] = trialResult;
        }
        else
        {
            gameLog["TrialResult"] = "NA";
        }

        loggingManager.Log("Game", gameLog);
    }

    public void EndGame()
    {
        interTrialTimer = 0f;
        if (inputWindow == InputWindowState.Open)
        {
            CloseInputWindow();
        }
        gameState = GameState.Stopped;
        GameData gameData = createGameData();
        onGameStateChanged.Invoke(gameData);
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
    */
}
