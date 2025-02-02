using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : Passive
{
    void Awake()
    {
        ratio1 = 0.2f;
        ratio2 = 1f;
        ratio3 = 1f;

        passiveName = "Détonation";
        fight_description = "Lorsqu'une unité adverse meurt, elle inflige %%1 dégats aux unités adjacentes";
    }
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnDeath += Detonation;
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        EventManager.OnDeath -= Detonation;
    }
    void Detonation(DeathEvent deathEvent){
        if(deathEvent.GetDeadUnit().GetTeam() != holder.GetTeam()){
            foreach (BaseUnit adjUnit in UnitManager.Instance.GetAdjacentUnits(deathEvent.GetDeadUnit()))
            {
                SpellManager.Instance.InflictDamage(GetOwner(), GetFinalDamages(ratio1), adjUnit);
            }
            
        }
    }
}