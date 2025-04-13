using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragmentation : Passive
{
    BaseUnit summonedUnit;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnDamage += Fragment;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.OnDamage -= Fragment;
    }
    void Fragment(DamageEvent damageEvent){
        if(damageEvent.GetTargetUnit() == holder && damageEvent.GetHealthAmount() != 0){
            //CreateUnit
        }
    }
}
