using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;
    public GameObject informationPanel;
    public TextMeshProUGUI unitNamePanel;
    public TextMeshProUGUI unitPowerPanel;
    public TextMeshProUGUI unitHealthPanel;
    public TextMeshProUGUI unitLevelPanel;
    public TextMeshProUGUI unitPassiveNamePanel;
    public TextMeshProUGUI unitPassiveDescriptionPanel;

    public GameObject spellSelector;
    public GameObject shade;

    public BaseUnit sourceUnit;
    private enum SpellChoice{CHARACTER, LEFT, RIGHT, UP, DOWN}
    private SpellChoice spellChoice;

    void Awake(){
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                CharacterSelection();
                break;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                SpellSelectionDisplay();
                break;
            default:
                Debug.Log(BattleManager.Instance.GetCurrentStatesSummary());
                break;
        }
        
    }
    void CharacterSelection(){
        Tile currentSelection = GridManager.Instance.GetMainSelection();

        if (Input.GetKeyDown(KeyCode.B)){
            if(currentSelection.GetUnit()!= null){
                if(currentSelection.GetUnit().GetTeam() == Team.Ally){
                    SpellSelectionActivation(true, currentSelection);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.N)){
            spellSelector.SetActive(false);
            shade.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(currentSelection.GetNextTile(Directions.UP) != null){
                currentSelection.GetNextTile(Directions.UP).Select();
                currentSelection.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(currentSelection.GetNextTile(Directions.DOWN) != null){
                currentSelection.GetNextTile(Directions.DOWN).Select();
                currentSelection.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(currentSelection.GetNextTile(Directions.LEFT) != null){
                currentSelection.GetNextTile(Directions.LEFT).Select();
                currentSelection.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            if(currentSelection.GetNextTile(Directions.RIGHT) != null){
                currentSelection.GetNextTile(Directions.RIGHT).Select();
                currentSelection.Unselect();
            }
        }

        BaseUnit currentUnit = currentSelection.GetUnit();
        if(currentUnit != null){
            informationPanel.SetActive(true);
            unitNamePanel.text = currentUnit.GetName();
            unitPowerPanel.text = "Puissance : " + currentUnit.GetFinalPower().ToString();
            unitHealthPanel.text = "PV : " + currentUnit.GetFinalHealth().ToString();
            unitLevelPanel.text = "Niveau : " + currentUnit.GetLevel().ToString();
            unitPassiveNamePanel.text = currentUnit.GetPassive().GetName();
            unitPassiveDescriptionPanel.text = currentUnit.GetPassive().GetFightDescription();
        }
        else{
            informationPanel.SetActive(false);
        }
    }

    void SpellSelectionActivation(bool activation, Tile currentSelection = null){
        spellSelector.SetActive(activation);
        shade.SetActive(activation);
        if (activation){
            spellSelector.transform.position = currentSelection.transform.position;
            sourceUnit = currentSelection.GetUnit();
            BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.VALIDATE);
        }
        else{
            sourceUnit = null;
            BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.CANCEL);
        }

    }
    void SpellSelectionDisplay(){
        //Display highlight
        spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);

        switch(spellChoice){
            case SpellChoice.CHARACTER:
                spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.LEFT:
                spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.RIGHT:
                spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.UP:
                spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.DOWN:
                spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }

        // Navigation au sein de la sélection
        switch(spellChoice){
            case SpellChoice.CHARACTER:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner attaque
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour à la sélection de personnages
                    SpellSelectionActivation(false);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                break;
            case SpellChoice.LEFT:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell gauche
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                break;
            case SpellChoice.RIGHT:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell à droite
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;
            case SpellChoice.UP:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell en haut
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;
            case SpellChoice.DOWN:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell en bas
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;

            default:
                break;
        }

        /*
        if (Input.GetKeyDown(KeyCode.B)){

        }
        if (Input.GetKeyDown(KeyCode.N)){

        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){

        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){

        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){

        }
        */

        /*
        BaseUnit currentUnit = currentSelection.GetUnit();
        if(currentUnit != null){
            informationPanel.SetActive(true);
            unitNamePanel.text = currentUnit.GetName();
            unitPowerPanel.text = "Puissance : " + currentUnit.GetFinalPower().ToString();
            unitHealthPanel.text = "PV : " + currentUnit.GetFinalHealth().ToString();
            unitLevelPanel.text = "Niveau : " + currentUnit.GetLevel().ToString();
            unitPassiveNamePanel.text = currentUnit.GetPassive().GetName();
            unitPassiveDescriptionPanel.text = currentUnit.GetPassive().GetFightDescription();
        }
        else{
            informationPanel.SetActive(false);
        }
        */
    }
}

public enum Directions {RIGHT, LEFT, UP, DOWN}
