using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2Mng : MonoBehaviour
{
    public static Game2Mng Instance;

    public Color colorChoice;
    public List<Color> colorPalette;

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
        colorChoice = Color.white;
    }

    public List<Color> GetColors (int count)
    {
        List<Color> colList = new List<Color>();
        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, colorPalette.Count);
            colList.Add(colorPalette[i]);
            colorPalette.RemoveAt(i);
        }

        for (int i = 0; i < colList.Count; i++)
        {
            colorPalette.Add(colList[i]);
        }
        return colList;
    }

    public void ChangeColor (int id)
    {
        colorChoice = colorPalette[id];
    }

}
