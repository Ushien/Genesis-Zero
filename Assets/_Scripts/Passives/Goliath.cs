using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goliath : Passive
{
    Modifier newModifier;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        newModifier = Instantiate(modifier);
        newModifier.Setup(gameObject, _healthBonus : ratio1, _duration : Modifier.Duration.Permanent);
        holder.AddGlobalModifier(newModifier);
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        holder.DeleteGlobalModifier(newModifier);
    }
}
