using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultPowerModifier : EffectModifier
{
    // Power modification amount.
    // 1.0 = origin power
    // 0.0 = no power
    // 0.1 = 10% of the origin power
    // 2.0 = 200% of the origin power
    private float power;

    public MultPowerModifier(Upgrade _origin, float _power)
    {
        SetOrigin(_origin);
        SetPower(_power);
    }

    private void SetPower(float _power)
    {
        power = _power;
    }
}