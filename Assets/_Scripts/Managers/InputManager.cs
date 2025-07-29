using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public PlayerInput GetPlayerInput()
    {
        return GetComponent<PlayerInput>();
    }
}
