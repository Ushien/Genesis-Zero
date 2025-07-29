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
    public bool isPaused { get; private set; }
    private PlayerInput playerInput;
    private string previousActionMap;

    void Awake()
    {
        Instance = this;

        playerInput = InputManager.Instance.GetPlayerInput();

        playerInput.actions["Pause"].performed += PauseGame;
        playerInput.actions["Unpause"].performed += UnpauseGame;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0.5f;
        previousActionMap = playerInput.currentActionMap.id.ToString();
        playerInput.SwitchCurrentActionMap("Pause");
    }

    public void UnpauseGame(InputAction.CallbackContext context)
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        playerInput.SwitchCurrentActionMap(previousActionMap);
    }
}
