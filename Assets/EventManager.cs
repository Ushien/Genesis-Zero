using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;
    public delegate void ClickAction();
    public delegate void SpellAction();
    public static event ClickAction OnClicked;
    public static event Action<BaseSpell> OnCast;
    void Awake(){
        Instance = this;
    }
    public void Click(){
        if (OnClicked != null){
            OnClicked();
        }
    }
    public void CastSpell(BaseSpell spell){
        if (OnCast != null){
            OnCast(spell);
        }
    }

}
