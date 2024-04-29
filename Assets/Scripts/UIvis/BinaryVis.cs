using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinaryVis : MonoBehaviour
{
    float fade = 0f;
    float fadeAdd = 1f;

    Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        img.color = Color.HSVToRGB(0f, 0.4f, fade);

        if (fade > 0f)
        {
            fade -= Time.deltaTime;
        }
    }

    public void PingImg (MotorImageryEvent newEvent)
    {
        if (newEvent == MotorImageryEvent.MotorImagery)
        fade = fadeAdd;
    }

}
