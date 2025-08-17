using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ArmesArgentées : Passive
{
    int damageCounter = 0;
    private BaseUnit originUnit;
    private ArmesArgentées originPassive;

    public void SetOriginUnit(BaseUnit _originUnit)
    {
        originUnit = _originUnit;
    }
    public void SetOriginPassive(ArmesArgentées _originPassive)
    {
        originPassive = _originPassive;
    }

    override public void Desactivate()
    {
        //
    }
    
    override public void Trigger1(){
        int finalDamages = 0;

        damageCounter += 1;
        while(damageCounter >= 3){
            finalDamages += GetFinalDamages(ratio1);
            damageCounter -= 3;
        }

        if (finalDamages > 0)
        {
            SpellManager.Instance.InflictDamage(originUnit, finalDamages, GetOwner());
            originPassive.Notify();
        }
    }

}
