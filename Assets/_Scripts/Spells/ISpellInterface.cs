using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellInterface
{
    void Cast(Tile target = null);
    public void SetSpell(BaseSpell baseSpell);
}
