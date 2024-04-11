using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    void Awake(){
        Instance = this;
    }
    public void Click(){
        if (OnClicked != null){
            OnClicked();
        }
    }

}
