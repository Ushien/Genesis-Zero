using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmesArgentées : Passive
{
    [SerializeField]
    private ScriptablePassive a_ArmesArgentees;

    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnDamage += Argent;
    }
    // Lorsque le passif disparaît, le désactive
    void OnDisable()
    {
        EventManager.OnDamage -= Argent;
    }
    void Argent(DamageEvent damageEvent){
        if(damageEvent.GetOriginUnit() == holder){
			if(!damageEvent.GetTargetUnit().HasPassive(a_ArmesArgentees)){
				// Ajoute le passif
                a_ArmesArgentees.SetupPassive(damageEvent.GetTargetUnit());

			}
            // Incrémente le passif
            damageEvent.GetTargetUnit().GetPassive(a_ArmesArgentees).Trigger1();
        }
    }
}
