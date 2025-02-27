using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Académique : Passive
{
    List<Modifier> modifiers = new List<Modifier>();
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.AfterCast += RisingPower;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.AfterCast -= RisingPower;
        holder.DeleteGlobalModifier(modifiers);
    }
    void RisingPower(AfterCastEvent castEvent){
        if(castEvent.GetSourceUnit() == holder && castEvent.GetCastedSpell().IsATechnique()){
            Modifier newModifier = Instantiate(modifier);
            newModifier.Setup(gameObject, ratio1, _duration : Modifier.Duration.Battle);
            modifiers.Add(newModifier);
            holder.AddGlobalModifier(newModifier);
        }
    }
}
