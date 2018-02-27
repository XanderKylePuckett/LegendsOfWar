﻿using UnityEngine;
public abstract class AbilityBase : MonoBehaviour
{
    [SerializeField]
    protected Effect m_effect;
    public Sprite abilityIcon;
    [SerializeField]
    protected float cooldownTime = 10.0f;
    [SerializeField]
    private GameObject cursor = null;
    [SerializeField]
    private Texture2D CursorIcon = null;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField]
    private Vector2 hotSpot = Vector2.zero;
    public string abilityDescEn = "", abilityDescJp = "", abilityNameEn = "Ability", abilityNameJp = "スペル";
    public float abilityCost = 10.0f;
    protected HeroInfo heroInfo;
    protected float cooldownTimer = 0.0f, skillTimer = 0.0f;
    protected bool abilityOn = false, abilityEnabled = true;
    public bool AbilityEnabled
    { set { abilityEnabled = value; } }
    public Effect Effect
    { get { return m_effect; } }
    public float Timer
    {
        get { return cooldownTimer; }
        set { cooldownTimer = value; }
    }
    public bool EnoughMana
    { get { return heroInfo.Mana >= abilityCost; } }
    public void TryCast()
    {
        if (GameManager.GameRunning)
            if (abilityEnabled)
                if (gameObject.activeInHierarchy)
                    if (cooldownTimer <= 0.0f)
                        if (heroInfo.UseMana(abilityCost))
                            AbilityActivate();
    }
    protected virtual void Start()
    {
        if ("" == m_effect.m_name)
            m_effect.m_name = "<n/a>";
        if (cooldownTime <= m_effect.m_duration)
            cooldownTime = m_effect.m_duration;
        heroInfo = GetComponentInParent<HeroInfo>();
        cursor = GameManager.TheCursor;
        if (CursorIcon)
            hotSpot.Set(CursorIcon.width * 0.5f, CursorIcon.height * 0.5f);
    }
    protected virtual void Update()
    {
        skillTimer -= Time.deltaTime;
        if (abilityOn && skillTimer <= 0.0f)
            AbilityDeactivate();
    }
    protected virtual void AbilityActivate()
    {
        abilityOn = true;
        cooldownTimer = cooldownTime;
        skillTimer = m_effect.m_duration;
        heroInfo.Deidle();
    }
    protected virtual void AbilityDeactivate()
    {
        abilityOn = false;
        skillTimer = 0.0f;
    }
    protected void ToggleCursor(bool _bool)
    {
        if (cursor)
            if (!(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.
                E) || Input.GetKey(KeyCode.R)))
                cursor.SetActive(_bool);
        if (_bool)
            Cursor.SetCursor(CursorIcon, hotSpot, cursorMode);
        else
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}