using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foi : Passive
{
    CharacterModifier newModifier;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        newModifier = Instantiate(modifier);
        newModifier.Setup(gameObject, _powerBonus : ratio1, _duration : CharacterModifier.Duration.Permanent);
        GetOwner().AddModifier(newModifier, GetOwner().Heal);
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        GetOwner().DeleteModifier(newModifier, GetOwner().Heal);
        // Retire le modificateur du personnage
    }
}
