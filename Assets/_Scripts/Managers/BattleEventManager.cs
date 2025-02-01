using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : MonoBehaviour
{
    public static BattleEventManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCastEvent(BaseUnit sourceUnit, BaseSpell castedSpell, Tile targetTile, bool animation = true){
        CastEvent newCastEvent = new CastEvent(sourceUnit, castedSpell, targetTile);
        BattleManager.Instance.AddEvent(newCastEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newCastEvent);
        }
    }

    public DamageEvent CreateDamageEvent(BaseUnit targetUnit, int amount, bool _armorDamages = false){
        return new DamageEvent(null, targetUnit, amount, _armorDamages);
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

    public void CreateHealEvent(BaseUnit targetUnit, int amount, bool animation = true){
        HealEvent newHealEvent = new HealEvent(targetUnit, amount);
        EventManager.Instance.UnitHealed(targetUnit);
        BattleManager.Instance.AddEvent(newHealEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newHealEvent);
        }
    }
}
