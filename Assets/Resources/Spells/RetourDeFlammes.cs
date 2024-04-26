using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetourDeFlammes : MonoBehaviour, ISpellInterface
{
    private BaseSpell spell = null;
    public void Cast(Tile target = null){
        //
    }

    public void SetSpell(BaseSpell baseSpell){
        spell = baseSpell;
    }
}
