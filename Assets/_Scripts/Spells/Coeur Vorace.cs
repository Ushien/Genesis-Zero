using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CoeurVorace : BaseSpell
{
    int healCount = 0;
    override public void Activate(){
        EventManager.AfterHeal += IncrementHeal;
    }

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties,_CoeurVorace);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _CoeurVorace_H, hyper: true);
    }

    private void _CoeurVorace(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Vampirisme), SpellType.Damage);
    }

    private void _CoeurVorace_H(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper:true)[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Vampirisme), SpellType.Damage);
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
        // TODO doit être modifié pour respecter le même système que les passifs
        EventManager.AfterHeal -= IncrementHeal;
    }
}

