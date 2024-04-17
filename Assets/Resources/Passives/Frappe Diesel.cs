using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrappeDiesel : Passive
{
    void Awake()
    {
        passiveName = "Frappe Diesel";
        fight_description = "Les attaques de base du personnage font des dégats supplémentaires et appliquent l'élément pyro.";
    }
    
    // Lorsque le passif est setup, l'active

    // Lorsque le passif disparaît, le désactive
}
