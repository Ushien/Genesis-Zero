using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmesArgentées : Passive
{
    [SerializeField]
    private ScriptablePassive a_ArmesArgentees;
    private List<Passive> passivesAdded = new List<Passive>();

    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        EventManager.OnDamage += Argent;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        EventManager.OnDamage -= Argent;
        foreach (Passive _passive in passivesAdded)
        {
            _passive.holder.DeletePassive(_passive);
        }
    }
    void Argent(DamageEvent damageEvent){
        if(damageEvent.GetOriginUnit() == holder){
			if(!damageEvent.GetTargetUnit().HasPassive(a_ArmesArgentees)){
				// Ajoute le passif
                Passive newPassive = a_ArmesArgentees.SetupPassive(damageEvent.GetTargetUnit());
                passivesAdded.Add(newPassive);
			}
            // Incrémente le passif
            damageEvent.GetTargetUnit().GetPassive(a_ArmesArgentees).Trigger1();
        }
    }
}
