using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

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
            currentSelection.GetNextTile(Directions.UP).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            currentSelection.GetNextTile(Directions.DOWN).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            currentSelection.GetNextTile(Directions.LEFT).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            currentSelection.GetNextTile(Directions.RIGHT).Select();
            currentSelection.Unselect();
        }

        string information = currentSelection.GetUnit().GetName();
        Debug.Log(information);
    }

}

public enum Directions {RIGHT, LEFT, UP, DOWN}
