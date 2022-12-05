using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Letter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private char _character;
    public UnityEvent<Letter> Clicked = new UnityEvent<Letter>();
    public bool IsPlaced = false;
    public char Character
    {
        get => _character;
        set
        {
            _character = value;
            _text.text = _character.ToString();
        }
    }

    public float Angle { 
        get => _angle;
        set {
            while (value < 0) value += 2 * Mathf.PI;
            while (value > 2 * Mathf.PI) value -= 2 * Mathf.PI;
            _angle = value; 
        } 
    }

    private float _angle;
    public float StartRadius;
    public float EndRadius;

    private float _radius;
    private Vector2 _defaultSize;
    private RectTransform _rectTransform;

    public void Init(char character, float angle, float startRadius, float endRadius)
    {
        Character = character;
        Angle = angle;
        StartRadius = startRadius;
        EndRadius = endRadius;
        _rectTransform = GetComponent<RectTransform>();
        _defaultSize = _rectTransform.rect.size;
        Clicked.AddListener(UpdateSize);
    }
    public void UpdateSize(Letter l)
    {
        if (!l.IsPlaced) _rectTransform.sizeDelta = _defaultSize;
    }
    public void SetPosition()
    {
        transform.localPosition = new Vector3(Mathf.Sin(Angle), Mathf.Cos(Angle), 0) * _radius;
    }
    public void UpdateRadius(float factor)
    {
        _radius = Mathf.Lerp(StartRadius, EndRadius, factor);
    }
    public void OnClick()
    {
        Clicked.Invoke(this);
    }
}