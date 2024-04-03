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
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            currentSelection.GetNextTile(KeyboardDirections.UP).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            currentSelection.GetNextTile(KeyboardDirections.DOWN).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            currentSelection.GetNextTile(KeyboardDirections.LEFT).Select();
            currentSelection.Unselect();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            currentSelection.GetNextTile(KeyboardDirections.RIGHT).Select();
            currentSelection.Unselect();
        }
    }

}

public enum KeyboardDirections {RIGHT, LEFT, UP, DOWN}
