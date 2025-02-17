using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;
using Unity.VisualScripting;

/// <summary>
/// Gestion du système d'évènements
/// </summary>

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;
    public delegate void AfterSpellAction();
    public static event Action<CastEvent> AfterCast;
    public delegate void BeforeSpellAction();
    public static event Action<CastEvent> BeforeCast;
    public delegate void AfterHealAction();
    public static event Action<HealEvent> AfterHeal;
    public delegate void DeathAction();
    public static event Action<DeathEvent> OnDeath;
    public delegate void DamageAction();
    public static event Action<DamageEvent> OnDamage;
    void Awake(){
        Instance = this;
    }

    public void AfterTechCast(CastEvent castEvent){
        if (AfterCast != null){
            AfterCast(castEvent);
        }
    }

    public void BeforeTechCast(CastEvent castEvent){
        if (BeforeCast != null){
            BeforeCast(castEvent);
        }
    }

    public void UnitHealed(HealEvent healEvent){
        if (AfterHeal != null){
            AfterHeal(healEvent);
        }
    }

    public void UnitDied(DeathEvent deathEvent){
        if (OnDeath != null){
            OnDeath(deathEvent);
        }
    }

    public void UnitDamaged(DamageEvent damageEvent){
        if (OnDamage != null){
            OnDamage(damageEvent);
        }
    }

}
