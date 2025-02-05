using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EndScreenManager : MonoBehaviour
{
    private bool win;
    public static EndScreenManager Instance;

    [SerializeField]
    private TextBox textbox;
    private List<TextBox> textboxs;

    void Awake()
    {
        Instance = this;
        textboxs = new List<TextBox>();
    }

    private void Update(){
        if(win){
            WinUpdate();
        }
        else{
            LoseUpdate();
        }
    }

    public void Setup(bool _win){
        win = _win;
        if(win){
            DisplayWin();
        }
        else{
            DisplayLose();
        }
    }

    private void DisplayWin(){
        //
    }

    private void DisplayLose(){
        TextBox text = Instantiate(textbox);
        textboxs.Add(text);
        text.CreateTextBox("Partie terminée. Appuyez sur B pour relancer une run.");
    }

    private void WinUpdate(){
        //
    }

    private void LoseUpdate(){
        // Si on appuie sur B on quitte l'écran
        if (Input.GetKeyDown(KeyCode.B)){
            GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.ENDPHASE);
        }
    }

    public void Out(){
        foreach (TextBox _textbox in textboxs)
        {
            Destroy(_textbox.gameObject);
        }
    }
}
