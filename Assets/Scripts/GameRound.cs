using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameRound : MonoBehaviour
{
    [SerializeField]private Transform _gameZone;
    [SerializeField]private Transform _wordPanel;
    [SerializeField] private GameObject _buttonsPanelNext;
    [SerializeField] private GameObject _buttonsPanelGameover;
    [SerializeField] private GameObject _helpButton;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private Letter _letterPrefab;
    [SerializeField] private Image _progress;

    [SerializeField] private float _startRadius = 10f;
    [SerializeField] private float _maxRadius = 50f;
    [SerializeField] private float _letterRadius = 50f;
    [SerializeField] private float _prepareTime = 1f;

    private float _time = 15;
    private float _timer;
    private int _helpCount = 2;

    private LevelSettings _levelSettings;
    private int _level = 4;
    private float _endRadius = 50f;
    private bool _isPlaying;
    private bool _helpUsed;
    private bool _helpShowed;
    private bool _canUseHelp;
    private bool _gameWon = false;

    private int _score = 0;
    private int _round = 0;

    private List<Letter> _availableLetters = new List<Letter>();

    public int Score { get => _score; 
        set
        {
            _score = value;
            _scoreText.text = $"{ _score}";
        }
    }

    public void StartRound(LevelSettings level)
    {
        _levelSettings = level;
        _helpCount = level.CharsPerHelp;
        _round = 0;
        StartRound();
    }
    public void StartRound()
    {
        if (!_gameWon) Score = 0;
        _level = _levelSettings.Level;
        _time = _levelSettings.SecondsPerRound(_round);
        _gameWon = false;
        _buttonsPanelNext.SetActive(false);
        _buttonsPanelGameover.SetActive(false);
        _helpButton.SetActive(false);
        _helpUsed = false;
        _helpShowed = false;
        _canUseHelp = true;
        string word = DataBase.GetWord(_level);
        Debug.Log(word);
        ClearField();
        Debug.Log(_availableLetters.Count);
        StartCoroutine(Game(word));
    }
    private IEnumerator Game(string word)
    {
        yield return new WaitForEndOfFrame();
        FillField(word);
        _isPlaying = true;
        StartCoroutine(UpdatePositions());
        yield return InitField();
        _timer = 1;
        yield return RoundTimer();
        _isPlaying = false;        
    }
    private void FillField(string word)
    {
        int cnt = word.Length;
        int index = 0;
        // b / a = sin()
        _endRadius = Mathf.Clamp(_letterRadius / Mathf.Sin(Mathf.PI / cnt), 0, _maxRadius);
        List<int> shuffledIndexes = new List<int>();
        for (int i = 0; i < cnt; i++) shuffledIndexes.Add(i);
        var rnd = new System.Random();
        shuffledIndexes = shuffledIndexes.OrderBy(item => rnd.Next()).ToList();
        foreach (char ch in word)
        {
            float angle = Mathf.PI * 2 * shuffledIndexes[index] / cnt;
            index++;
            Letter l = Instantiate(_letterPrefab, _gameZone);
            l.Clicked.AddListener(OnLetterClick);
            l.Init(ch, angle, _startRadius, _endRadius);
            _availableLetters.Add(l);
        }
    }

    private void ClearField()
    {
        for(int i = _availableLetters.Count - 1; i > -1; i--)
        {
            Letter l = _availableLetters[i];
            Destroy(l.gameObject);
        }
        _availableLetters.Clear();
    }
    private IEnumerator InitField()
    {
        _progress.fillAmount = 1;
        float t = 0;
        while (t < 1)
        {
            foreach (Letter l in _availableLetters)
            {
                l.UpdateRadius(t);
            }
            t += Time.deltaTime / _prepareTime; 
            yield return new WaitForEndOfFrame();

        }
        foreach (Letter l in _availableLetters)
        {
            l.UpdateRadius(1);
        }
        yield return null;
    }
    private IEnumerator UpdatePositions()
    {
        while (_isPlaying)
        {
            foreach (Letter l in _availableLetters)
            {
                if (!l.IsPlaced)
                {
                    l.SetPosition();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator RoundTimer()
    {
        while (_timer > 0)
        {
            _progress.fillAmount = _timer;
            _timer -= Time.deltaTime / _time;
            yield return null;
            if (!_isPlaying) yield break;
            if(_timer < .5f)
            {
                if(_canUseHelp && !_helpShowed)
                {
                    _helpButton.SetActive(true);
                    _helpShowed = true;
                }
                else if (!_canUseHelp && _helpShowed)
                {
                    _helpButton.SetActive(false);
                    _helpShowed = false;
                }
            }
        }
        _progress.fillAmount = 0;
        _timer = 0;
        GameLost();
        yield return null;
    }
    private void OnLetterClick(Letter letter)
    {
        if (!_isPlaying) return;
        letter.IsPlaced = !letter.IsPlaced;

        letter.transform.SetParent(letter.IsPlaced ? _wordPanel : _gameZone);
        if (CheckAnswer())
        {
            GameWon();
        }
    }

    public void OnHelpClick()
    {
        _helpUsed = true;
        _canUseHelp = false;
        for (int i = 0; i < Mathf.Min(_helpCount,_availableLetters.Count); i++)
        {
            OnLetterClick(_availableLetters[i]);
        }
    }

    private void GameLost()
    {
        _helpButton.SetActive(false); 
        foreach (Letter l in _availableLetters)
        {
            l.IsPlaced = true;
            l.transform.SetParent(_wordPanel);
        }
        int i = 0;
        foreach (Letter l in _availableLetters)
        {
            l.transform.SetSiblingIndex(i);
            i++;
        }
        _buttonsPanelGameover.SetActive(true);
    }
    private void GameWon()
    {
        _gameWon = true;
        Score += _levelSettings.GetReward(_timer, _round);
        _round++;
        _buttonsPanelNext.SetActive(true);
    }
    private bool CheckAnswer()
    {
        char[] wordChars = new char[_availableLetters.Count];
        _canUseHelp = !_helpUsed;
        foreach (Letter l in _availableLetters)
        {
            if (l.IsPlaced) _canUseHelp = false;
        }
        for (int i = 0; i <  _availableLetters.Count; i++)
        {
            Letter l = _availableLetters[i];
            if (!l.IsPlaced) return false;
            int index = Mathf.Clamp(l.transform.GetSiblingIndex(), 0, _availableLetters.Count - 1);
            wordChars[index] = l.Character;
        }
        string word = new string(wordChars);
        if (DataBase.CheckWord(word))
        {
            _isPlaying = false;
            return true;
        }
        return false;
    }
}
