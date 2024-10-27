using System.Collections.Generic;

public class BattleTurn{
        private int turnNb;
        private List<BattleEvent> battleEvents;
        private List<Instruction> instructions;

        public BattleTurn(int _turnNb){
                turnNb = _turnNb;
                battleEvents = new List<BattleEvent>();
                instructions = new List<Instruction>();
        }

        public int GetTurnNb(){
                return turnNb;
        }

        public List<BattleEvent> GetBattleEvents(){
                return battleEvents;
        }

        public List<Instruction> GetInstructions(){
                return instructions;
        }

        public void AddEvent(BattleEvent newEvent){
                battleEvents.Add(newEvent);
        }
        public void AddInstruction(Instruction newInstruction){
                instructions.Add(newInstruction);
        }
        public void SetInstructions(List<Instruction> newInstructions){
                instructions = newInstructions;
        }
}