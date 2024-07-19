using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

/// <summary>
/// Gestion du système d'évènements
/// </summary>

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;
    public delegate void AfterSpellAction();
    public static event Action<BaseSpell, Tile> AfterCast;
    public delegate void BeforeSpellAction();
    public static event Action<BaseSpell, Tile> BeforeCast;
    public delegate void DeathAction();
    public static event Action<BaseUnit> OnDeath;
    void Awake(){
        Instance = this;
    }

    public void AfterTechCast(BaseSpell spell, Tile targetTile){
        if (AfterCast != null){
            AfterCast(spell, targetTile);
        }
    }

    public void BeforeTechCast(BaseSpell spell, Tile targetTile){
        if (BeforeCast != null){
            BeforeCast(spell, targetTile);
        }
    }

    public void UnitDied(BaseUnit unit){
        if (OnDeath != null){
            OnDeath(unit);
        }
    }

}
