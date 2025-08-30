using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Elysium : Passive
{
    CharacterModifier newModifier;
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        newModifier = Instantiate(modifier);
        newModifier.Setup(gameObject, _duration : CharacterModifier.Duration.Permanent, _properties : new List<Properties>(){Properties.Curatif});
        GetOwner().AddGlobalModifier(newModifier);
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        GetOwner().DeleteGlobalModifier(newModifier);
    }
}
