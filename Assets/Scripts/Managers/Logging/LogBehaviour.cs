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


    public struct GameData
    {
        //public int frameCount;
        public float confidence;
        public float classificationThreshold;
        public float bciThresholdBuffer;
        public float bufferSize;
    }

    LoggingManager loggingManager;

    private void Start()
    {
        loggingManager = FindObjectOfType<LoggingManager>();
    }

    private void LogGameEvent ()
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>()
        {
            
        };
    }
    /*
    public void GenerateUIDs()
    {
        sessionID = Md5Sum(System.DateTime.Now.ToString(SystemInfo.deviceUniqueIdentifier + "yyyy:MM:dd:HH:mm:ss.ffff").Replace(" ", "").Replace("/", "").Replace(":", ""));
        deviceID = SystemInfo.deviceUniqueIdentifier;
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
    */
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
