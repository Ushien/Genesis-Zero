using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foi : Passive
{
    Modifier newModifier;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        newModifier = Instantiate(modifier);
        newModifier.Setup(gameObject, _powerBonus : ratio1, _duration : Modifier.Duration.Permanent);
        holder.AddModifier(newModifier, holder.Heal);
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        holder.DeleteModifier(newModifier, holder.Heal);
        // Retire le modificateur du personnage
    }
}
