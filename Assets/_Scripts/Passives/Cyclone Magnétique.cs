using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CycloneMagnetique : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnEndTurn += Cyclone;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate(){
        EventManager.OnEndTurn -= Cyclone;
    }
    void Cyclone(EndTurnEvent endTurnEvent)
    {
        if (endTurnEvent.GetTeam() == Team.Enemy)
        {
            foreach (BaseUnit unit in UnitManager.Instance.GetUnits(Team.Enemy))
            {
                SpellManager.Instance.InflictDamage(GetOwner(), GetFinalDamages(ratio1), unit);
            }
            Notify();
        }
    }
}
