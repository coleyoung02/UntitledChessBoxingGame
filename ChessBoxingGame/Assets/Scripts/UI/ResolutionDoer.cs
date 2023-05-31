using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionDoer : MonoBehaviour
{
    private int resolutionScale = 24;
    static int gridSize = 24;
    private int width = 16;
    private int height = 9;
    [SerializeField] private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        int w = Display.main.systemWidth / (width * gridSize + 1);
        Debug.Log("Width " + w);
        int h = Display.main.systemHeight / (height * gridSize + 1);
        Debug.Log("Height " + h);
        resolutionScale = Math.Min(w, h);
        if (resolutionScale <= 2) resolutionScale = 2;
        Screen.SetResolution(width * resolutionScale * gridSize, height * resolutionScale * gridSize, false);
        //text.text = w.ToString() + ", " + h.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void debugPrint(float input)
    {
        Debug.Log(input);
    }
    public void changeRes(int increment)
    {
        if (resolutionScale + increment > 1)
        {
            resolutionScale += increment;
        }
        Screen.SetResolution(width * resolutionScale * gridSize, height * resolutionScale * gridSize, false);
    }
}
