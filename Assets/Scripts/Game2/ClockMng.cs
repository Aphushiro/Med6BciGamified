using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClockMng : MonoBehaviour
{
    public Image[] pieImg = new Image[4];

    public RectTransform handParent;

    float handSpeed = 24f;

    bool isResting = false;

    int currentlyOn = 0;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (isResting) { return; }
        float rotAmount = - (Time.deltaTime * (360f / handSpeed));
        handParent.Rotate(Vector3.forward, rotAmount);

        float zRot = handParent.rotation.eulerAngles.z;
        //Debug.Log(zRot);
        // Reset colors
        for (int i = 0; i < pieImg.Length; i++)
        {
            pieImg[i].color = new Color(1, 1, 1, 0.5f);
        }

        // Set color of choice
        if (zRot < 45 || zRot > 315f) // Top (0)
        {
            pieImg[0].color = Color.white;
            currentlyOn = 0;
        } else if (zRot < 315f && zRot > 225f) // Right (1)
        {
            pieImg[1].color = Color.white;
            currentlyOn = 1;
        } else if (zRot < 225f && zRot > 135f) // Bottom (2)
        {
            pieImg[2].color = Color.white;
            currentlyOn = 2;
        } else // Left (3)
        {
            pieImg[3].color = Color.white;
            currentlyOn = 3;
        }
    }

    public void WaitForResting ()
    {
        if (isResting) { return; }
        isResting = true;
        StartCoroutine(Rest());

    }

    IEnumerator Rest ()
    {
        yield return new WaitForSeconds(Game2Mng.Instance.restTime);
        isResting = false;
    }

    public int GetClockState ()
    {
        WaitForResting();
        return currentlyOn;
    }
}
