using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    public UnityEvent<int> Clicked = new UnityEvent<int>();

    [SerializeField] private Transform _gameZone;
    [SerializeField] private Transform _wordPanel1;
    [SerializeField] private Transform _wordPanel2;

    [SerializeField] private Letter _letterPrefab;

    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _letterRadius = 100f;
    [SerializeField] private float _spinSpeed = 10f;

    private List<Letter> _availableLetters = new List<Letter>();
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        string levels = "45678";
        RectTransform field = _gameZone.GetComponent<RectTransform>();
        _radius = Mathf.Max(field.rect.width / 2 - _letterRadius, _letterRadius);
        _radius = Mathf.Max(_letterRadius / Mathf.Sin(Mathf.PI / levels.Length), _radius);
        FillField(levels);
        FillWord("circle", _wordPanel1);
        FillWord("word", _wordPanel2);
    }
    private void FillField(string counts)
    {
        int cnt = counts.Length;
        int index = 0;
        foreach(char ch in counts)
        {
            float angle = Mathf.PI * 2 * index / cnt;
            index++;
            Letter l = Instantiate(_letterPrefab, _gameZone);
            l.Clicked.AddListener(OnLetterClick);
            l.Init(ch, angle, _radius, _radius); 
            l.UpdateRadius(1);
            l.SetPosition();
            _availableLetters.Add(l);
        }
    }
    private void FillWord(string word, Transform parent)
    {
        foreach (char ch in word)
        {
            Letter l = Instantiate(_letterPrefab, parent);
            l.Character = ch;
        }
    }
    private void Update()
    {
        for(int i = 0; i < _availableLetters.Count; i++)
        {
            _availableLetters[i].Angle += Time.deltaTime * _spinSpeed * Mathf.Deg2Rad;
            _availableLetters[i].SetPosition();
        }
    }

    private void OnLetterClick(Letter letter)
    {
        int level = letter.Character - '0';
        Clicked.Invoke(level);
    }
}
