using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Académique : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.AfterCast += RisingPower;
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        EventManager.AfterCast -= RisingPower;
    }
    void RisingPower(BaseSpell spell, Tile targetTile){
        if(spell.GetOwner() == holder){
            holder.ModifyPower(ratio1);
        }
    }
}
