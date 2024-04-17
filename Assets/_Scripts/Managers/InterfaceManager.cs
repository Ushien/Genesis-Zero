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
        Tile currentSelection = GridManager.Instance.GetMainSelection();

        if (Input.GetKeyDown(KeyCode.Q)){
            
        }
        if (Input.GetKeyDown(KeyCode.W)){
    
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

}

public enum Directions {RIGHT, LEFT, UP, DOWN}
