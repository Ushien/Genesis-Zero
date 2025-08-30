using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyModifier : EffectModifier
{
    private Properties property;

    public PropertyModifier(Upgrade _origin, Properties _property)
    {
        SetOrigin(_origin);
        SetProperty(_property);
    }

    private void SetProperty(Properties _property)
    {
        property = _property;
    }
}
