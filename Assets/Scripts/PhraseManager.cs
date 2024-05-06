using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhraseManager : MonoBehaviour
{
    public static PhraseManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    [SerializeField] private List<string> restPhrases = new List<string>
    {
        "Good job, now rest",
        "Take a break, you've earned it",
        "Relax and recharge",
        // Add more phrases as needed
    };

    [SerializeField] private List<string> activePhrases = new List<string>
    {
        "Concentrate now",
        "Engage",
        "Use all your power now",
        // Add more phrases as needed
    };
        
    public string GetActivePhrase()
    { ;
        int index = Random.Range(0, activePhrases.Count);
        return activePhrases[index];
    }
        
    public string GetRestPhrase()
    { ;
        int index = Random.Range(0, restPhrases.Count);
        return restPhrases[index];
    }
}
