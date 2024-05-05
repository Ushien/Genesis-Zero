using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;
    public delegate void SpellAction();
    public static event Action<BaseSpell> OnCast;
    public delegate void DeathAction();
    public static event Action<BaseUnit> OnDeath;
    void Awake(){
        Instance = this;
    }

    public void TechCasted(BaseSpell spell){
        if (OnCast != null){
            OnCast(spell);
        }
    }
    public void UnitDied(BaseUnit unit){
        if (OnDeath != null){
            OnDeath(unit);
        }
    }

}
