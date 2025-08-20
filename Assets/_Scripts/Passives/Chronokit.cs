using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chronokit : Passive
{
    // Lorsque le passif est setup, l'active
    override public void Activate()
    {
        BattleManager.Instance.howManyEndturnEffects *= 2;
    }
    // Lorsque le passif disparaît, le désactive
    override public void Desactivate()
    {
        BattleManager.Instance.howManyEndturnEffects /= 2;
    }
}
