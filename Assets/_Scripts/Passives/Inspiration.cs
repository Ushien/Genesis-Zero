using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspiration : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.BeforeCast+= _Inspiration;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.BeforeCast -= _Inspiration;
    }
    void _Inspiration(BeforeCastEvent castEvent){
        if(castEvent.GetCastedSpell().GetOwner() == GetOwner() && castEvent.GetTargetTile() != GetOwner().GetTile() && castEvent.GetTargetTile().GetUnit().GetTeam() == GetOwner().GetTeam()){
            castEvent.GetTargetTile().GetUnit().Cleanse();
        }
    }
}
