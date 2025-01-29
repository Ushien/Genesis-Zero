using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CoeurVorace : BaseSpell
{
    int healCount = 0;
    void Awake(){
        SetRatio(1, 1f);
        SetRatio(2, 1f);

        EventManager.AfterHeal += IncrementHeal;
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _CoeurVorace);
    }

    private void _CoeurVorace(Tile targetTile){   
        SpellManager.Instance.InflictDamage(GetOwner(), GetDamages(), targetTile.GetUnit(), new List<Properties>(){Properties.Vampirisme});
    }

    private float GetDamages(){
        return GetRatio()[0] * GetOwner().GetFinalPower() + GetRatio()[1] * GetOwner().GetFinalPower() * healCount;
    }

    public void IncrementHeal(BaseUnit unit){
        if(unit == GetOwner()){
            healCount += 1;
        }
    }

    void OnDisable()
    {
        EventManager.AfterHeal -= IncrementHeal;
    }
}

