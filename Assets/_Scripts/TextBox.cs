using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public GameObject nameBox;
    public TextMeshProUGUI nameText;


    public Vector3 dialogPosition = new Vector4(0f, -380f, 1450f, 225f);
    // Start is called before the first frame update
    public void Awake()
    {
        mainText = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        nameBox = transform.GetChild(1).gameObject;
        nameText = nameBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void CreateTextBox(string textToDisplay){
        mainText.text = textToDisplay;
        gameObject.SetActive(true);
    }

    public void CreateTalkBox(string textToDisplay, string displayName){
        mainText.text = textToDisplay;
        nameBox.SetActive(true);
        nameText.text = displayName;
        gameObject.SetActive(true);
    }
}
