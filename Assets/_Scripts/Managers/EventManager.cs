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
    public static event Action<AfterCastEvent> AfterCast;
    public delegate void BeforeSpellAction();
    public static event Action<BeforeCastEvent> BeforeCast;
    public delegate void AfterHealAction();
    public static event Action<HealEvent> AfterHeal;
    public delegate void DeathAction();
    public static event Action<DeathEvent> OnDeath;
    public delegate void DamageAction();
    public static event Action<DamageEvent> OnDamage;
    public delegate void ArmorGainAction();
    public static event Action<ArmorGainEvent> OnArmorGain;
    public delegate void SummonAction();
    public static event Action<SummonEvent> OnSummon;
    void Awake(){
        Instance = this;
    }

    public void AfterTechCast(AfterCastEvent afterCastEvent){
        if (AfterCast != null){
            AfterCast(afterCastEvent);
        }
    }

    public void BeforeTechCast(BeforeCastEvent BeforeCastEvent){
        if (BeforeCast != null){
            BeforeCast(BeforeCastEvent);
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

    public void UnitGainedArmor(ArmorGainEvent armorGainEvent){
        if (OnArmorGain != null){
            OnArmorGain(armorGainEvent);
        }
    }

    public void UnitSummoned(SummonEvent summonEvent){
        if (OnSummon != null){
            OnSummon(summonEvent);
        }
    }
}
