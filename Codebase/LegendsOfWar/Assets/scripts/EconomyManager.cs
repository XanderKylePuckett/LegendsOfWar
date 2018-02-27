﻿using UnityEngine;
public class EconomyManager : MonoBehaviour
{
    [SerializeField]
    private float startingAmount = 100.0f, waveAmount = 10.0f;
    public delegate void goldChangedEvent();
    public event goldChangedEvent OnGainGold;
    private static EconomyManager instance = null;
    private float blueTeamGold = 0.0f, redTeamGold = 0.0f;
    public static EconomyManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<EconomyManager>();
                if (!instance)
                    instance = new GameObject("EconomyManager").AddComponent<EconomyManager>();
            }
            return instance;
        }
    }
    public float BlueGold
    { get { return blueTeamGold; } }
    public void StartingGame()
    {
        blueTeamGold = redTeamGold = startingAmount;
    }
    public void GiveGold(Team team, float amount)
    {
        if (amount <= 0.0f)
            return;
        switch (team)
        {
            case Team.RED_TEAM:
                redTeamGold += amount;
                break;
            case Team.BLUE_TEAM:
                blueTeamGold += amount;
                break;
            default:
                break;
        }
        OnGainGold?.Invoke();
    }
    public bool TakeGold(Team team, float amount)
    {
        switch (team)
        {
            case Team.RED_TEAM:
                if (redTeamGold - amount >= 0.0f)
                {
                    redTeamGold -= amount;
                    return true;
                }
                break;
            case Team.BLUE_TEAM:
                if (blueTeamGold - amount >= 0.0f)
                {
                    blueTeamGold -= amount;
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }
    public void NewWave()
    {
        GiveGold(Team.BLUE_TEAM, waveAmount);
        GiveGold(Team.RED_TEAM, waveAmount);
    }
    private void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        if (this == instance)
            instance = null;
    }
}