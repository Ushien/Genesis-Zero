using System.Collections.Generic;

public class BattleTurn{
        private int turnNb;
        private List<BattleEvent> battleEvents;
        private List<Instruction> instructions;

        bool archived = false;

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

        public void RemoveInstruction(int index){
                if(instructions.Count > index && index >= 0){
                instructions[index].GetSourceUnit().GiveInstruction(false);
                //Destroy(playerInstructions[index].gameObject);
                instructions.RemoveAt(index);
                }
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

        public void ArchiveTurn(bool isArchived = true){
                archived = isArchived;
        }
}