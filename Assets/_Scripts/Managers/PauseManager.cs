using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField]
    GameObject pauseMenu;
    public bool isPaused { get; private set; }
    private PlayerInput playerInput;
    private string previousActionMap;
    List<Locale> localesList;
    int currentLocaleId;

    void Awake()
    {
        Instance = this;

        playerInput = InputManager.Instance.GetPlayerInput();

        playerInput.actions["Pause"].performed += PauseGame;
        playerInput.actions["Unpause"].performed += UnpauseGame;
        playerInput.actions["MoveMenu"].performed += MoveInMenu;

        localesList = LocalizationSettings.Instance.GetAvailableLocales().Locales;
        currentLocaleId = localesList.IndexOf(LocalizationSettings.Instance.GetSelectedLocale());
    }

    private void MoveInMenu(InputAction.CallbackContext context)
    {
        float xMove = context.ReadValue<Vector2>().x;
        if (xMove > 0)
        {
            if (currentLocaleId == localesList.Count - 1)
            {
                currentLocaleId = 0;
            }
            else
            {
                currentLocaleId += 1;
            }
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLocaleId];
        }
        else if (xMove < 0)
        {
            if (currentLocaleId == 0)
            {
                currentLocaleId = localesList.Count - 1;
            }
            else
            {
                currentLocaleId -= 1;
            }
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLocaleId];
        }
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
