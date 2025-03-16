using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TextBox : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public GameObject nameBox;
    public TextMeshProUGUI nameText;

    public Canvas canvas;


    public Vector3 dialogPosition = new Vector4(0f, -380f, 1450f, 225f);
    // Start is called before the first frame update
    public void Awake()
    {
        mainText = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        nameBox = transform.GetChild(1).gameObject;
        nameText = nameBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        canvas = InterfaceManager.Instance.GetUI();
        transform.SetParent(canvas.transform);
        transform.localScale = Vector3.one;
    }

    public void CreateTextBox(string textToDisplay){
        mainText.text = textToDisplay;
        transform.localPosition = new Vector3(0f, 0f, transform.position.z);
    }

    public void CreateTalkBox(string textToDisplay, string displayName){
        mainText.text = textToDisplay;
        nameBox.SetActive(true);
        nameText.text = displayName;
        gameObject.SetActive(true);
    }
}
