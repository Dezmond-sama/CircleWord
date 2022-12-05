using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSettings
{
    [SerializeField] private int _level = 4;
    [SerializeField] private int _charsPerHelp = 1;
    [SerializeField] private float _secondsPerRound = 15f;
    [SerializeField] private float _timeByLevelMultiplier = 0.95f;
    [SerializeField] private int _maxReward = 150;
    [SerializeField] private int _minReward = 10;
    [SerializeField] private int _maxRewardByLevelAppend = 20;
    [SerializeField] private int _minRewardByLevelAppend = 10;
    [SerializeField] private AnimationCurve _rewardCurve = AnimationCurve.Linear(1, 1, 0, 0);

    public float SecondsPerRound(int round){
        return _secondsPerRound * Mathf.Pow(_timeByLevelMultiplier, round);
    }

    public int Level { get => _level; }
    public int CharsPerHelp { get => _charsPerHelp; }

    private int GetMinReward(int round)
    {
        return _minReward + round * _minRewardByLevelAppend;
    }

    private int GetMaxReward(int round)
    {
        return _maxReward + round * _maxRewardByLevelAppend;
    }

    public int GetReward(float timer, int round)
    {
        return Mathf.FloorToInt((_rewardCurve.Evaluate(Mathf.Clamp01(1 - timer)) * (GetMaxReward(round) - GetMinReward(round)) + GetMinReward(round)));
    }

}
