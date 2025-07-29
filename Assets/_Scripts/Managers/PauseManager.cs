using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField]
    GameObject pauseMenu;
    public bool isPaused { get; private set;}
    void Awake()
    {
        Instance = this;

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Pause.performed += PauseGame;
        playerInputActions.Pause.Unpause.performed += UnpauseGame;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void UnpauseGame(InputAction.CallbackContext context)
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }
}
