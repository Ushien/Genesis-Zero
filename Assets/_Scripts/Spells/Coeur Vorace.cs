using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CoeurVorace : BaseSpell
{
    int healCount = 0;
    void Awake(){
        EventManager.AfterHeal += IncrementHeal;
    }

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties,_CoeurVorace);
    }

    private void _CoeurVorace(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Vampirisme), SpellType.Damage);
    }

    override public int GetFinalDamages(float _ratio){
        return Tools.Ceiling(_ratio * GetOwner().GetFinalPower() + GetRatio()[1] * GetOwner().GetFinalPower() * healCount);
    }

    public void IncrementHeal(HealEvent healEvent){
        if(healEvent.GetTargetUnit() == GetOwner()){
            healCount += 1;
        }
    }

    void OnDisable()
    {
        EventManager.AfterHeal -= IncrementHeal;
    }
}

