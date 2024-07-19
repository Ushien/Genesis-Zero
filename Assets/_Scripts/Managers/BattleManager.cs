using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

/// <summary>
/// Contient toutes les méthodes relatives au moteur de combat
/// </summary>

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;

    //Game states
    public enum BattleState {OUT, START, TURN, END, WON, LOST}
    public enum TurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum PlayerActionChoiceState {OUT, START, CHARACTER_SELECTION, SWITCH_CHARACTER, SPELL_SELECTION, TARGET_SELECTION, VALIDATED_ACTION, OTHER_STATE, EXIT}

    public enum Machine{BATTLESTATE, PLAYERTURNSTATE, PLAYERACTIONCHOICESTATE}
    public enum Trigger {VALIDATE, CANCEL, FORWARD, EMPTY}
    public enum TeamTurn{OUT, ALLY, ENEMY}

    public BattleState battleState;
    public TurnState turnState;
    public PlayerActionChoiceState playerActionChoiceState;
    public TeamTurn teamTurn;

    public Instruction emptyInstruction;

    private List<Instruction> playerInstructions;
    public List<BattleEvent> currentTurnEvents = new List<BattleEvent>();

    public List<List<BattleEvent>> archivedTurnEvents = new List<List<BattleEvent>>();

    public int nTurn = 0;

    private bool inAnimation = false;

    void Awake(){
        Instance = this;
        CleanPlayerInstructions();
    }

    void Update(){

        if(!inAnimation){
            ChangeState(Machine.PLAYERACTIONCHOICESTATE, Trigger.EMPTY);
            ChangeState(Machine.PLAYERTURNSTATE, Trigger.EMPTY);
            ChangeState(Machine.BATTLESTATE, Trigger.EMPTY);
        }
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
                ChangeTurnState(trigger);
                break;
            case Machine.BATTLESTATE:
                ChangeBattleState(trigger);
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

    private void ChangeTurnState(Trigger trigger){
        switch (turnState)
        {
            case TurnState.OUT:
                // Nothing happens unless it receives an FORWARD signal
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Do stuff if needed
                        turnState = TurnState.START;
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.START:
                // Do stuff
                // Start turn effects
                turnState = TurnState.ACTION_CHOICE;
                if(teamTurn == TeamTurn.ALLY){
                    ChangeState(Machine.PLAYERACTIONCHOICESTATE, Trigger.FORWARD);
                }
                break;

            case TurnState.ACTION_CHOICE:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        turnState = TurnState.APPLY_ACTIONS;
                        break;
                    
                    default:
                        if(teamTurn == TeamTurn.ENEMY){
                            playerInstructions = AIManager.Instance.GetAIOrders(ConvertTeamTurn(teamTurn));
                            turnState = TurnState.APPLY_ACTIONS;
                        }
                        break;
                }
                break;

            case TurnState.APPLY_ACTIONS:
                ApplyInstructions();
                // Sauvegarder l'historique d'instructions
                // TODO
                // Clean les instructions actuelles
                CleanPlayerInstructions();

                turnState = TurnState.END;
                break;

            case TurnState.END:
                // Do stuff
                // End turn effects
                turnState = TurnState.OUT;
                ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
                break;
            default:
                break;
        }
    }
    private void ChangeBattleState(Trigger trigger){
        switch (battleState){
            case BattleState.OUT:
                break;
            case BattleState.START:
                break;
            case BattleState.TURN:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Check if game is over
                        if(isGameOver()){
                            battleState = BattleState.END;
                        }
                        else{
                            EndTurnEffects();
                            AnimateElements();
                            CleanTurnEvents();
                            SwitchCurrentTeam();
                            nTurn ++;
                            ChangeTurnState(Trigger.FORWARD);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case BattleState.END:
                break;
            case BattleState.WON:
                break;
            case BattleState.LOST:
                break;
            default:
                break;
        }
    }

    private bool isGameOver()
    {
        //TODO Implémenter
        return false;
    }

    private void SwitchCurrentTeam()
    {
        if(teamTurn == TeamTurn.ALLY){
            teamTurn = TeamTurn.ENEMY;
        }
        else{
            teamTurn = TeamTurn.ALLY;
        }
    }

    private void CleanPlayerInstructions(){
        playerInstructions = new List<Instruction>();
        UnitManager.Instance.MakeUnitsActive();
    }

    public void DebugSetState(){
        teamTurn = TeamTurn.ALLY;

        battleState = BattleState.TURN;
        turnState = TurnState.ACTION_CHOICE;
        playerActionChoiceState = PlayerActionChoiceState.OUT;
    }

    public string GetCurrentStatesSummary(){
        string currentStates = "";
        currentStates = currentStates + "BattleState: " + battleState + "\n";
        currentStates = currentStates + "TurnState: " + turnState + "\n";
        currentStates = currentStates + "PlayerActionChoiceState: " + playerActionChoiceState + "\n";
        return currentStates;

    }
    public BattleState GetBattleState(){
        return battleState;
    }

    public TurnState GetTurnState(){
        return turnState;
    }

    public PlayerActionChoiceState GetPlayerActionChoiceState(){
        return playerActionChoiceState;
    }
    private void StartBattle(){
        teamTurn = TeamTurn.ALLY;
        battleState = BattleState.START;
        turnState = TurnState.OUT;
        playerActionChoiceState = PlayerActionChoiceState.OUT;
    }

    public void EndTurnEffects(){
        UnitManager.Instance.ApplyEndTurnEffects(ConvertTeamTurn(teamTurn));
    }

    private void CleanTurnEvents(){
        archivedTurnEvents.Add(currentTurnEvents);
        currentTurnEvents = new List<BattleEvent>();
    }

    public Team ConvertTeamTurn(TeamTurn teamTurn){
        switch (teamTurn)
        {
            case TeamTurn.ALLY:
                return Team.Ally;
                
            case TeamTurn.ENEMY:
                return Team.Enemy;

            default:
                return Team.Both;
        }
    }

    public Instruction CreateInstruction(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile){
        Instruction new_instruction = new Instruction(source_unit, spell_to_cast, target_tile);
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
        if(playerInstructions.Count > index && index >= 0){
            playerInstructions[index].GetSourceUnit().GiveInstruction(false);
            //Destroy(playerInstructions[index].gameObject);
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

    public void AddEvent(BattleEvent _event){
        currentTurnEvents.Add(_event);
    }

    public void SetInAnimation(bool value){
        inAnimation = value;
    }

    public void AnimateElements(){
        SetInAnimation(true);
        var task = AnimationManager.Instance.Animate(currentTurnEvents);
    }
}
