using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Académique : Passive
{
    void Awake()
    {
        passiveName = "Académique";
        fight_description = "Après avoir lancé une technique, le lanceur gagne 10% de puissance";
    }
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnCast += RisingPower;
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        EventManager.OnCast -= RisingPower;
    }
    void RisingPower(BaseSpell spell){
        if(spell.owner == holder){
            holder.ModifyPower(0.1f);
        }
    }
}
