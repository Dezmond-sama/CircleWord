using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameRound _gamePanel;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private LevelSettings[] levels;


    private void Start()
    {
        OnButtonMenu();
    }
    
    private void OnEnable()
    {
        _mainMenu.Clicked.AddListener(OnButtonStart);
    }
    private void OnDisable()
    {
        _mainMenu.Clicked.RemoveListener(OnButtonStart);
    }
    public void OnButtonMenu()
    {
        _mainMenu.gameObject.SetActive(true);
        _gamePanel.gameObject.SetActive(false);
    }
    public void OnButtonStart(int level)
    {
        _mainMenu.gameObject.SetActive(false);
        _gamePanel.gameObject.SetActive(true);
        _gamePanel.StartRound(GetSettings(level));
    }
    private LevelSettings GetSettings(int level)
    {
        foreach (var item in levels)
        {
            if (item.Level == level) return item;
        }
        return new LevelSettings();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(0);
        }
    }
}
