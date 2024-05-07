using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPassive : Passive
{
    void Awake()
    {
        passiveName = "Pas de passif";
        fight_description = "Ce passif ne fait rien";
    }

    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        //
    }

    void OnDisable()
    {
        //
    }
    
    // Lorsque le passif disparaît, le désactive

    void ShoutName()
    {
        Debug.Log(holder.GetName());
    }
}
