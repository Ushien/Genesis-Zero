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

    // Lorsque le passif disparaît, le désactive
}
