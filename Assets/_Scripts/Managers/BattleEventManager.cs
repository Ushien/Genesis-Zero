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

    public void CreateDamageEvent(BaseUnit targetUnit, int amount, bool _armorDamages = false, bool animation = true){
        DamageEvent newDamageEvent = new DamageEvent(targetUnit, amount, _armorDamages);
        BattleManager.Instance.AddEvent(newDamageEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newDamageEvent);
        }
    }

    public void CreateArmorGainEvent(BaseUnit targetUnit, int amount, bool animation = true){
        ArmorGainEvent newArmorGainEvent = new ArmorGainEvent(targetUnit, amount);
        BattleManager.Instance.AddEvent(newArmorGainEvent);
        if(animation){
            AnimationManager.Instance.addAnimation(newArmorGainEvent);
        }
    }
}
