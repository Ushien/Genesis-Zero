using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    private bool win;
    public static EndScreenManager Instance;

    [SerializeField]
    private TextBox textbox;

    void Awake()
    {
        Instance = this;
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
        text.CreateTextBox("Partie terminée. Appuyez sur B pour relancer une run.");
    }

    private void WinUpdate(){
        //
    }

    private void LoseUpdate(){
        // Si on appuie sur B on quitte l'écran
    }
}
