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
        if (Input.GetKeyDown(KeyCode.D)){
            GridManager.Instance.GetMainSelection().GetNextTile(KeyboardDirections.RIGHT).Select();
        }
    }

}

public enum KeyboardDirections {RIGHT, LEFT, UP, DOWN}
