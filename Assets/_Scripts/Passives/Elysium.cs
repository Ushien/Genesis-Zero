using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Elysium : Passive
{
    Modifier newModifier;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        newModifier = Instantiate(modifier);
        newModifier.Setup(gameObject, _duration : Modifier.Duration.Permanent, _properties : new List<Properties>(){Properties.Curatif});
        holder.AddGlobalModifier(newModifier);
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        holder.DeleteGlobalModifier(newModifier);
    }
}
