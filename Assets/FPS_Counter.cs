using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;
    private void Awake()
    {
        frameDeltaTimeArray = new float[50];   
    }

    // Update is called once per frame
    void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.unscaledDeltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

        GetComponent<TextMeshProUGUI>().text = "FPS : " + Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    private float CalculateFPS(){
        float total = 0f;
        foreach(float deltaTime in frameDeltaTimeArray){
            total += deltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }
}
