using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
        public BaseUnit emptyUnit;
        public BaseSpell emptySpell;
        public Tile emptyTile;

        public BaseUnit source;
        public BaseSpell spell;
        public Tile target;

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

        public BaseUnit GetSourceUnit(){
            return source;
        }
        public BaseSpell GetSpell(){
            return spell;
        }
        public Tile GetTargetTile(){
            return target;
        }
}
