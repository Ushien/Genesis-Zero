using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] public GameObject _highlight;
    public int x_position;
    public int y_position;

    public bool main_selection = false;

    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter() {
        main_selection = true;
        _highlight.SetActive(true);
    }

    void OnMouseDown() {
        // if(GameManager.Instance.Gamestate != GameState.HeroesTurn) return;
    }

    void OnMouseExit() {
        main_selection = false;
        _highlight.SetActive(false);
    }
}