using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;

    //Game states
    public enum BattleState {OUT, START, TURN, END, WON, LOST}
    public enum TurnState {OUT, PLAYERTURN, ENEMYTURN}
    public enum PlayerTurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum EnemyTurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum PlayerActionChoiceState {OUT, START, CHARACTER_SELECTION, SWITCH_CHARACTER, SPELL_SELECTION, TARGET_SELECTION, VALIDATED_ACTION, OTHER_STATE, EXIT}

    public enum Machine{BATTLESTATE, TURNSTATE, PLAYERTURNSTATE, ENEMYTURNSTATE, PLAYERACTIONCHOICESTATE}
    public enum Trigger {VALIDATE, CANCEL, LEFT, RIGHT, UP, DOWN, FORWARD}

    public BattleState battleState;
    public TurnState turnState;
    public PlayerTurnState playerTurnState;
    public EnemyTurnState enemyTurnState;
    public PlayerActionChoiceState playerActionChoiceState;

    public Instruction emptyInstruction;

    private List<Instruction> playerInstructions;

    public int nTurn = 0;

    void Awake(){
        Instance = this;
        playerInstructions = new List<Instruction>();
    }

    void Update(){
        ChangePlayerActionChoiceState(Trigger.FORWARD);
        ChangePlayerTurnState(Trigger.FORWARD);
    }

    public void LaunchBattle(List<Tuple<Vector2, ScriptableUnit, int>> ally_composition, List<Tuple<Vector2, ScriptableUnit, int>> enemy_composition){
        
        GridManager.Instance.GenerateGrids();
        UnitManager.Instance.spawnAllies(ally_composition);
        UnitManager.Instance.SpawnEnemies(enemy_composition);

        StartBattle();
    }

    public void ChangeState(Machine machine, Trigger trigger){

        switch (machine)
        {
            case Machine.PLAYERACTIONCHOICESTATE:
                ChangePlayerActionChoiceState(trigger);
                break;
            case Machine.PLAYERTURNSTATE:
                ChangePlayerTurnState(trigger);
                break;
            default:
                break;
        }

        
    }

    private void ChangePlayerActionChoiceState(Trigger trigger){
        switch(playerActionChoiceState){

            case PlayerActionChoiceState.CHARACTER_SELECTION:

                switch (trigger){
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        break;
                    case Trigger.CANCEL:
                        // if(un des alliés a déjà sélectionné son instruction)
                        //     on retire la dernière instruction et on reste dans l'état
                        break;
                    default:
                        break;
                }
                break;
            case PlayerActionChoiceState.SPELL_SELECTION:
                switch (trigger){
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.TARGET_SELECTION;
                        break;
                    case Trigger.CANCEL:
                        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                        break;
                    default:
                        break;
                }
                break;
            case PlayerActionChoiceState.TARGET_SELECTION:
                switch (trigger)
                {
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.VALIDATED_ACTION;
                        break;
                    case Trigger.CANCEL:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        break;

                    default:
                        break;
                }
                break;

            case PlayerActionChoiceState.VALIDATED_ACTION:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        if(UnitManager.Instance.DidEveryCharacterGaveInstruction()){
                            playerActionChoiceState = PlayerActionChoiceState.EXIT;
                        }
                        else{
                            playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                        }
                        break;
                    default:
                        break;
                }


            break;
            

            case PlayerActionChoiceState.EXIT:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Do stuff
                        playerActionChoiceState = PlayerActionChoiceState.OUT;
                        break;
                    default:
                        break;
                }

            break;

        }
    }

    private void ChangePlayerTurnState(Trigger trigger){

    }

    public void DebugSetState(){
        battleState = BattleState.TURN;
        turnState = TurnState.PLAYERTURN;
        playerTurnState = PlayerTurnState.ACTION_CHOICE;
        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
    }

    public string GetCurrentStatesSummary(){
        string currentStates = "";
        currentStates = currentStates + "BattleState: " + battleState + "\n";
        currentStates = currentStates + "TurnState: " + turnState + "\n";
        currentStates = currentStates + "PlayerTurnState: " + playerTurnState + "\n";
        currentStates = currentStates + "EnemyTurnState: " + enemyTurnState + "\n";
        currentStates = currentStates + "PlayerActionChoiceState: " + playerActionChoiceState + "\n";
        return currentStates;

    }
    public BattleState GetBattleState(){
        return battleState;
    }

    public TurnState GetTurnState(){
        return turnState;
    }

    public PlayerTurnState GetPlayerTurnState(){
        return playerTurnState;
    }

    public EnemyTurnState GetEnemyTurnState(){
        return enemyTurnState;
    }

    public PlayerActionChoiceState GetPlayerActionChoiceState(){
        return playerActionChoiceState;
    }
    private void StartBattle(){
        battleState = BattleState.START;

        //Setup stuff

        //Start the first turn
        NextTurn();

    }

    private void StartTurn(){
        turnState = TurnState.PLAYERTURN;

        // Do things

        
    }

    [ContextMenu("Tour suivant")]
    public void NextTurn(){
        nTurn ++;
        StartTurn();
    }

    public Instruction CreateInstruction(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile){
        Instruction new_instruction = Instantiate(emptyInstruction);

        // L'associer au tour actuel
        new_instruction.Setup(source_unit, spell_to_cast, target_tile);
        return new_instruction;
    }

    public void AssignInstruction(Instruction instruction){
        AddInstruction(instruction);
        instruction.GetSourceUnit().GiveInstruction(true);
    }

    private void AddInstruction(Instruction instruction){
        playerInstructions.Add(instruction);
    }

    public int CountInstructions(){
        return playerInstructions.Count;
    }

    public bool AreInstructionsFull(){
        int registeredInstructionCount = 0;
        foreach (BaseUnit unit in UnitManager.Instance.GetUnits(Team.Ally))
        {
            foreach (Instruction instruction in playerInstructions){
                if(instruction.GetSourceUnit() == unit){
                    registeredInstructionCount ++;
                }
            }
        }
        return registeredInstructionCount == UnitManager.Instance.CountUnits(Team.Ally);
    }
}
