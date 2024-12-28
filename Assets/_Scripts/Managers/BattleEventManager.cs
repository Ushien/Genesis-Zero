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

    public void CreateCastEvent(BaseUnit sourceUnit, BaseSpell castedSpell, Tile targetTile){
        CastEvent newCastEvent = new CastEvent(sourceUnit, castedSpell, targetTile);
        BattleManager.Instance.AddEvent(newCastEvent);
        AnimationManager.Instance.addAnimation(newCastEvent);
    }

    public void CreateDamageEvent(BaseUnit targetUnit, int amount, bool _armorDamages = false){
        DamageEvent newDamageEvent = new DamageEvent(targetUnit, amount, _armorDamages);
        BattleManager.Instance.AddEvent(newDamageEvent);
        AnimationManager.Instance.addAnimation(newDamageEvent);
    }
}
