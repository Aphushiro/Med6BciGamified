using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2Mng : MonoBehaviour
{
    public static Game2Mng Instance;
    ClockMng clock;

    public Color colorChoice;
    public List<Color> colorPalette;

    public GameObject bucketObj;
    int bucketCount = 0;

    public float restTime = 8f;
    public bool isResting = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        colorChoice = colorPalette[0];
        clock = GetComponentInChildren<ClockMng>();
        Time.fixedDeltaTime = 0.03f;
    }

    public void OnResting (int state, float stateVal)
    {
        if (stateVal <= 0f && Time.time < 7f) { return; }
        bucketCount++;
        int clockState = state;
        colorChoice = colorPalette[clockState];

        int orient = 1;
        if (bucketCount%2 == 1) { orient = -1; }

        Vector2 bucketPos = new Vector2(-6.5f * orient, 3.5f);
        GameObject bucket = Instantiate(bucketObj, bucketPos, Quaternion.identity);
        BucketPref bucketCs = bucket.GetComponent<BucketPref>();
        bucketCs.dir = orient;
    }
}
