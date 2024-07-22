using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foi : Passive
{
    void Awake()
    {
        ratio1 = 0.15f;

        passiveName = "Foi";
        fight_description = "L'unité reçoit un bonus de __1% sur tous les soins qu'elle reçoit";
    }
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        modifier = Instantiate(modifier);
        modifier.Setup(_powerBonus : ratio1);
        holder.AddModifier(modifier, holder.Heal);
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        holder.DeleteModifier(modifier, holder.Heal);
        // Retire le modificateur du personnage
    }
}
