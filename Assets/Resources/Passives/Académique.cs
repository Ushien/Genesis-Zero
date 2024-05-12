using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Académique : Passive
{
    void Awake()
    {
        ratio1 = 0.1f;

        passiveName = "Académique";
        fight_description = "Après avoir lancé une technique, le lanceur gagne __1% de puissance";
    }
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
        if(spell.owner == holder){
            holder.ModifyPower(0.1f);
        }
    }
}
