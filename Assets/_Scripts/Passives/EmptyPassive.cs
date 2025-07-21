using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPassive : Passive
{
    void Awake()
    {
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
        //
    }
}
