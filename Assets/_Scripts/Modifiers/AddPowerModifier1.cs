using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add or delete fix damages/heal from an effect
// Ex: A 140 damages-fireball without modifier does 155 damages with a 15 amount AddPowerModifier
public class AddPowerModifier : EffectModifier
{
    private int amount;

    public AddPowerModifier(Upgrade _origin, int _amount)
    {
        SetOrigin(_origin);
        SetPower(_amount);
    }

    private void SetPower(int _power)
    {
        amount = _power;
    }
}