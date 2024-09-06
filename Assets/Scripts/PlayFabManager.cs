using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using YG;

public class PlayFabManager : MonoBehaviour
{
    [SerializeField] private GameObject _row;
    [SerializeField] private Transform _rowsParent;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TMP_Text _message;

    [SerializeField] private GameManager _gameManager;

    private string _username;
    private string _password;
    private bool _isHasAccount = false;

    private void Start()
    {
        LoadLoginAndPassword();
    }

    public void Register()
    {
        if (_passwordInput.text.Length < 6)
        {
            if(YandexGame.lang == "ru")
            {
                _message.text = "Пароль слишком короткий!";
            }
            else if(YandexGame.lang == "en")
            {
                _message.text = "The password is too short!";
            }
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Username = _emailInput.text,
            Password = _passwordInput.text,
            DisplayName = _emailInput.text,
            RequireBothUsernameAndEmail = false
        };
        _username = request.Username;
        _password = request.Password;
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void SaveLoginAndPassword(string username, string password)
    {
        PlayerPrefs.SetString("Username", _username);
        PlayerPrefs.SetString("Password", _password);
        PlayerPrefs.Save();
    }

    private void LoadLoginAndPassword()
    {
        if(PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            _gameManager.BG.SetActive(false);
            _username = PlayerPrefs.GetString("Username");
            _password = PlayerPrefs.GetString("Password");
            Debug.Log(_username + " " + _password);
            _isHasAccount = true;
            Login();
        }
        else
        {
            _gameManager._firstPanel.SetActive(true);
        }
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SaveLoginAndPassword(_username, _password);
        _isHasAccount = true;
        Login();
        Debug.Log("Успешная регистрация!");
        if (YandexGame.lang == "ru")
        {
            _message.text = "Регистрация выполнена";
        }
        else if (YandexGame.lang == "en")
        {
            _message.text = "Registration completed";
        }
    }

    public void Login()
    {
        if (_isHasAccount)
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = _username,
                Password = _password
            };
            PlayFabClientAPI.LoginWithPlayFab(request, OnSuccess, OnError);
        }
        else
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = _emailInput.text,
                Password = _passwordInput.text
            };
            _username = request.Username;
            _password = request.Password;
            PlayFabClientAPI.LoginWithPlayFab(request, OnSuccess, OnError);
        }
    }

    private void OnSuccess(LoginResult result)
    {
        SaveLoginAndPassword(_username, _password);
        Debug.Log("Вход успешно выполнен!");
        if (YandexGame.lang == "ru")
        {
            _message.text = "Вход выполнен";
        }
        else if (YandexGame.lang == "en")
        {
            _message.text = "Login done";
        }
        _nameText.text = _username;
        _gameManager.BG.SetActive(false);
        _gameManager._firstPanel.SetActive(false);
    }

    public void SubmitButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = _gameManager._field.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayName, OnError);
        _gameManager._firstPanel.SetActive(false);
        _gameManager.BG.SetActive(false);
        GetLeaderboard();
    }

    private void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Имя обновлено!");
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Ошибка входа!");
        Debug.Log(error.GenerateErrorReport());
        if(error.GenerateErrorReport() == "/Client/LoginWithPlayFab: User not found")
        {
            if (YandexGame.lang == "ru")
            {
                _message.text = "Неверный логин или пароль";
            }
            else if (YandexGame.lang == "en")
            {
                _message.text = "Invalid login or password";
            }
        }
        else if(error.GenerateErrorReport() == "/Client/LoginWithPlayFab: Invalid username or password")
        {
            if (YandexGame.lang == "ru")
            {
                _message.text = "Неверный логин или пароль";
            }
            else if (YandexGame.lang == "en")
            {
                _message.text = "Invalid login or password";
            }
        }
        else if(error.GenerateErrorReport() == "/Client/LoginWithPlayFab: Invalid input parameters")
        {
            if (YandexGame.lang == "ru")
            {
                _message.text = "Пользователь не существует";
            }
            else if (YandexGame.lang == "en")
            {
                _message.text = "User does not exist";
            }
        }
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdated, OnError);
    }

    private void OnLeaderboardUpdated(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Успешное обновление лидерборда!");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach(Transform item in _rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in result.Leaderboard)
        {
            GameObject newGO = Instantiate(_row, _rowsParent);
            TMP_Text[] texts = newGO.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
        }
    }
}
