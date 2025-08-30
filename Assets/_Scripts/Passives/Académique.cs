using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Académique : Passive
{
    List<CharacterModifier> modifiers = new List<CharacterModifier>();
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.AfterCast += RisingPower;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.AfterCast -= RisingPower;
        GetOwner().DeleteGlobalModifier(modifiers);
    }
    void RisingPower(AfterCastEvent castEvent){
        if (castEvent.GetSourceUnit() == GetOwner() && castEvent.GetCastedSpell().IsATechnique())
        {
            CharacterModifier newModifier = Instantiate(modifier);
            newModifier.Setup(gameObject, ratio1, _duration: CharacterModifier.Duration.Battle);
            modifiers.Add(newModifier);
            GetOwner().AddGlobalModifier(newModifier);
            Notify();
        }
    }
}
