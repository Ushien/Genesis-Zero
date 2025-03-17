using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benediction : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Benediction);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Benediction_H, hyper: true);
    }

    private void _Benediction(Tile targetTile, List<Properties> properties = null){

        Modifier _modifier = Instantiate(baseModifier);
        _modifier.Setup(gameObject, _powerBonus : GetRatio()[0], _turns : 3, _duration : Modifier.Duration.Battle);

        targetTile.GetUnit().GetAttack().AddModifier(_modifier);
    }

    private void _Benediction_H(Tile targetTile, List<Properties> properties = null){

        Modifier _modifier = Instantiate(SpellManager.Instance.GetModifier());
        _modifier.Setup(gameObject, _powerBonus : GetRatio(hyper:true)[0], _turns : 3, _duration : Modifier.Duration.Battle);

        targetTile.GetUnit().GetAttack().AddModifier(_modifier);
    }
}