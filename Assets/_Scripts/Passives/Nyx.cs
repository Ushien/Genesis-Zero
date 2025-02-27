using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nyx : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.AfterCast += Nocturne;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.AfterCast -= Nocturne;
    }
    void Nocturne(AfterCastEvent castEvent){
        if(castEvent.GetSourceUnit() == holder && castEvent.GetCastedSpell().IsAnAttack()){
            foreach (BaseSpell spell in holder.GetSpells())
            {
                if(spell.GetScriptableSpell() != castEvent.GetCastedSpell().GetScriptableSpell()){
                    spell.ModifyCooldown(+1);
                }
            }
        }
    }
}
