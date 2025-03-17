using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AssautCybernétique : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _AssautCybernétique);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _AssautCybernétique_H, hyper: true);
    }

    private void _AssautCybernétique(Tile targetTile, List<Properties> properties = null){
        for (int i = 0; i < GetRatio()[0]; i++)
        {
            GetOwner().Attack(targetTile, properties);
        }
    }

    private void _AssautCybernétique_H(Tile targetTile, List<Properties> properties = null){
        for (int i = 0; i < GetRatio(hyper : true)[0]; i++)
        {
            GetOwner().Attack(targetTile, properties);
        }
    }
}
