using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
        public BaseUnit emptyUnit;
        public BaseSpell emptySpell;
        public Tile emptyTile;

        private BaseUnit source;
        private BaseSpell spell;
        private Tile target;

        void Awake(){
            source = emptyUnit;
            spell = emptySpell;
            target = emptyTile;
        }
        public void Setup(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile){
            source = source_unit;
            spell = spell_to_cast;
            target = target_tile;
        }
}
