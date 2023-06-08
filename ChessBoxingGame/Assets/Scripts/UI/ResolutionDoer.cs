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
    private bool isFullScreen = false;
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
        //Debug.Log(input);
    }
    public void changeRes(int increment)
    {
        int nRes = resolutionScale + increment;
        if (nRes > 1 && width * nRes * gridSize < Display.main.systemWidth && height * nRes * gridSize < Display.main.systemHeight)
        {
            resolutionScale += increment;
            Screen.SetResolution(width * resolutionScale * gridSize, height * resolutionScale * gridSize, false);
        }
        else if (width * nRes * gridSize == Display.main.systemWidth && height * nRes * gridSize == Display.main.systemHeight)
        {
            resolutionScale += increment;
            Screen.SetResolution(width * resolutionScale * gridSize, height * resolutionScale * gridSize, true);
        }
        
    }
}
