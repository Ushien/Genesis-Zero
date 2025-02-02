using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : MonoBehaviour
{
    public static BattleEventManager Instance;
    // Start is called before the first frame update
    void Awake(){
        Instance = this;
    }
    public void CreateCastEvent(BaseUnit sourceUnit, BaseSpell castedSpell, Tile targetTile, bool animation = true){
        CastEvent newCastEvent = new CastEvent(sourceUnit, castedSpell, targetTile);
        BattleManager.Instance.AddEvent(newCastEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newCastEvent);
        }
    }

    public DamageEvent CreateDamageEvent(BaseUnit targetUnit, int health_amount, int armor_amount){
        return new DamageEvent(null, targetUnit, health_amount, armor_amount);
    }

    public DamageEvent CreateDamageEvent(BaseUnit originUnit, BaseUnit targetUnit, int health_amount, int armor_amount){
        return new DamageEvent(originUnit, targetUnit, health_amount, armor_amount);
    }

    public void ApplyDamageEvent(DamageEvent damageEvent, bool animation = true){
        EventManager.Instance.UnitDamaged(damageEvent);
        BattleManager.Instance.AddEvent(damageEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(damageEvent);
        }
    }

    public void CreateArmorGainEvent(BaseUnit targetUnit, int amount, bool animation = true){
        ArmorGainEvent newArmorGainEvent = new ArmorGainEvent(targetUnit, amount);
        BattleManager.Instance.AddEvent(newArmorGainEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newArmorGainEvent);
        }
    }

    public HealEvent CreateHealEvent(BaseUnit originUnit, BaseUnit targetUnit, int amount, bool animation = true){
        return new HealEvent(originUnit, targetUnit, amount);
    }
    
    public void ApplyHealEvent(HealEvent healEvent, bool animation = true){
        EventManager.Instance.UnitHealed(healEvent);
        BattleManager.Instance.AddEvent(healEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(healEvent);
        }
    }

    public DeathEvent CreateDeathEvent(BaseUnit deadUnit, Tile deathTile, bool animation = true){
        return new DeathEvent(deadUnit, deathTile, null);
    }

    public void ApplyDeathEvent(DeathEvent deathEvent, bool animation = true){
        EventManager.Instance.UnitDied(deathEvent);
        BattleManager.Instance.AddEvent(deathEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(deathEvent);
        }
    }
}
