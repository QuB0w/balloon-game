using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using PlayFab;
using YG;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private int _scorePerSecond = 1;
    [SerializeField] private Transform _player;

    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _pause;
    [SerializeField] private GameObject _death;
    [SerializeField] private GameObject _leaderboard;
    public GameObject _firstPanel;
    public GameObject BG;

    [SerializeField] private Button _confirmButton;
    public TMP_InputField _field;

    public float Speed;
    public int Money;
    public string PlayerName;

    public int _time;
    private PlayFabManager _playfabManager;
    private SpawnManager _spawnManager;
    
    private void Start()
    {
        Time.timeScale = 0;
        _field.shouldHideMobileInput = false;
        _playfabManager = GetComponentInChildren<PlayFabManager>();
        _spawnManager = GetComponentInChildren<SpawnManager>();
        if(PlayFabClientAPI.IsClientLoggedIn())
        {
            _playfabManager.Login();
        }
        
        StartCoroutine(wait());
        StartCoroutine(wait2());
    }

    public void OnClickPlay()
    {
        _menu.SetActive(false);
        _game.SetActive(true);
        Time.timeScale = 1;
    }

    public void OnClickMenuLeaderboard()
    {
        _leaderboard.SetActive(false);
    }

    public void OnClickPause()
    {
        _pause.SetActive(true);
        Time.timeScale = 0;
    }

    private void RestartGame()
    {
        _death.SetActive(false);
        _scorePerSecond = 1;
        _player.position = new Vector3(0, -4.5f, 0);
        Speed = 7;
        _spawnManager._timeToSpawnObstacles = 2;
        Money = 0;
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacles");
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coins");
        for (int i = 0; i < obstacles.Length; i++)
        {
            Destroy(obstacles[i]);
        }
        for (int i = 0; i < coins.Length; i++)
        {
            Destroy(coins[i]);
        }
    }

    public void OnClickMenu()
    {
        _playfabManager.GetLeaderboard();
        _pause.SetActive(false);
        _menu.SetActive(true);
        _game.SetActive(false);
        RestartGame();
        Time.timeScale = 0;
    }

    public void OnClickContinue()
    {
        _pause.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickConfirm()
    {
        PlayerName = _field.text;
        _playfabManager.Login();
        _firstPanel.SetActive(false);
    }

    private void Update()
    {
        _scoreText.text = Money.ToString();
        _time = Convert.ToInt32(Time.time);
    }

    private IEnumerator wait2()
    {
        yield return new WaitForSeconds(10f);
        _scorePerSecond += 1;
        if(Speed < 14)
        {
            Speed += 0.5f;
        }
        if (_spawnManager._timeToSpawnObstacles > 1f)
        {
            _spawnManager._timeToSpawnObstacles -= 0.05f;
        }
        StartCoroutine(wait2());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        Money += _scorePerSecond;
        StartCoroutine(wait());
    }

    public void OnClickRestart()
    {
        RestartGame();
        Time.timeScale = 1;
    }

    public void OnClickLeaderboard()
    {
        _playfabManager.GetLeaderboard();
        _leaderboard.SetActive(true);
    }

    public void Death()
    {
        _playfabManager.SendLeaderboard(Money);
        _playfabManager.GetLeaderboard();
        _death.SetActive(true);
        Time.timeScale = 0;
        YandexGame.FullscreenShow();
    }
}
