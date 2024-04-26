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
    public enum Trigger {VALIDATE, CANCEL, LEFT, RIGHT, UP, DOWN, FORWARD, EMPTY}

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
        CleanPlayerInstructions();
    }

    void Update(){
        ChangeState(Machine.PLAYERACTIONCHOICESTATE, Trigger.EMPTY);
        ChangeState(Machine.PLAYERTURNSTATE, Trigger.EMPTY);
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
            case PlayerActionChoiceState.OUT:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Do stuff if needed
                        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                        break;
                    default:
                        break;
                }
                break;

            case PlayerActionChoiceState.CHARACTER_SELECTION:

                switch (trigger){
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        break;
                    case Trigger.CANCEL:
                        CancelLastInstruction();
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
                    case Trigger.EMPTY:
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
                    case Trigger.EMPTY:
                        // Do stuff
                        playerActionChoiceState = PlayerActionChoiceState.OUT;
                        ChangeState(Machine.PLAYERTURNSTATE, Trigger.FORWARD);
                        break;
                    default:
                        break;
                }

            break;

        }
    }

    private void ChangePlayerTurnState(Trigger trigger){
        switch (playerTurnState)
        {
            case PlayerTurnState.ACTION_CHOICE:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        playerTurnState = PlayerTurnState.APPLY_ACTIONS;
                        break;
                    
                    default:
                        break;
                }
                break;
            case PlayerTurnState.APPLY_ACTIONS:
                ApplyInstructions();
                // Sauvegarder l'historique d'instructions
                // TODO
                // Clean les instructions actuelles
                CleanPlayerInstructions();
                //TODO faudra changer ça c'est temporaire
                playerTurnState = PlayerTurnState.ACTION_CHOICE;
                ChangeState(Machine.PLAYERACTIONCHOICESTATE, Trigger.FORWARD);
                break;
            default:
                break;
        }
    }

    private void CleanPlayerInstructions(){
        playerInstructions = new List<Instruction>();
        UnitManager.Instance.MakeUnitsActive();
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

    public void CancelLastInstruction(){
        RemoveInstruction(playerInstructions.Count - 1);
    }

    private void RemoveInstruction(int index){
        if(playerInstructions.Count > index){
            playerInstructions[index].GetSourceUnit().GiveInstruction(false);
            Destroy(playerInstructions[index].gameObject);
            playerInstructions.RemoveAt(index);
        }
    }

    private void AddInstruction(Instruction instruction){
        playerInstructions.Add(instruction);
    }

    private void ApplyInstructions(){
        foreach (Instruction instruction in playerInstructions)
        {
            ApplyInstruction(instruction);            
        }
    }

    private void ApplyInstruction(Instruction instruction){
        instruction.GetSpell().Cast(instruction.GetTargetTile());
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
