using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField]
    GameObject pauseMenu;
    private bool stateChanged = false;
    public bool isPaused { get; private set; }
    private PlayerInput playerInput;
    private string previousActionMap;

    void Update()
    {
        stateChanged = false;
    }
    void Awake()
    {
        Instance = this;

        playerInput = InputManager.Instance.GetPlayerInput();

        playerInput.actions["Pause"].performed += PauseGame;
        playerInput.actions["Unpause"].performed += UnpauseGame;
        /*
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Pause.started += PauseGame;
        playerInputActions.Pause.Unpause.started += UnpauseGame;
        */
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (!stateChanged && !isPaused)
        {
            pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0.5f;
            stateChanged = true;
            previousActionMap = playerInput.currentActionMap.id.ToString();
            playerInput.SwitchCurrentActionMap("Pause");
        }
    }

    public void UnpauseGame(InputAction.CallbackContext context)
    {
        if (!stateChanged && isPaused)
        {
            pauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
            stateChanged = true;
            playerInput.SwitchCurrentActionMap(previousActionMap);
        }
    }
}
