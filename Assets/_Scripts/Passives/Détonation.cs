using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnDeath += Detonation;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate(){
        EventManager.OnDeath -= Detonation;
    }
    void Detonation(DeathEvent deathEvent)
    {
        if(deathEvent.GetDeadUnit().GetTeam() != holder.GetTeam()){
            foreach (BaseUnit adjUnit in UnitManager.Instance.GetAdjacentUnits(deathEvent.GetDeadUnit()))
            {
                SpellManager.Instance.InflictDamage(GetOwner(), GetFinalDamages(ratio1), adjUnit);
            }
            
        }
    }
}