using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : Passive
{
    float ratio = 0.2f;
    void Awake()
    {
        passiveName = "Détonation";
        fight_description = "Lorsqu'un unité adverse meurt, elle inflige de légers dégats aux unités adjacentes";
    }
    // Lorsque le passif est setup, l'active
    void OnEnable()
    {
        EventManager.OnDeath += Detonation;
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        EventManager.OnDeath -= Detonation;
    }
    void Detonation(BaseUnit unit){
        if(unit.GetTeam() != holder.GetTeam()){
            foreach (BaseUnit adjUnit in UnitManager.Instance.GetAdjacentUnits(unit))
            {
                float finalDamages = ratio * holder.finalPower;
                SpellManager.Instance.InflictDamage(finalDamages, adjUnit);
            }
            
        }
    }
}