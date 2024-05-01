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
    }

    public void OnMiEvent ()
    {

        if (isResting) { return; }
        bucketCount++;
        int clockState = clock.GetClockState();
        colorChoice = colorPalette[clockState];

        int orient = 1;
        if (bucketCount%2 == 1) { orient = -1; }

        Vector2 bucketPos = new Vector2(-6f * orient, 3.8f);
        GameObject bucket = Instantiate(bucketObj, bucketPos, Quaternion.identity);
        BucketPref bucketCs = bucket.GetComponent<BucketPref>();
        bucketCs.dir = orient;
        StartCoroutine(BucketRest());
    }

    IEnumerator BucketRest ()
    {
        yield return new WaitForSeconds(restTime);
        isResting = false;
    }
}
