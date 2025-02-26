using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goliath : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        modifier = Instantiate(modifier);
        modifier.Setup(_healthBonus : ratio1);
        holder.AddGlobalModifier(modifier);
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        holder.DeleteGlobalModifier(modifier);
    }
}
