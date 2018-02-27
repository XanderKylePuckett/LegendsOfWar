﻿using UnityEngine;
using UnityEngine.UI;
public class HudScript : MonoBehaviour
{
    [SerializeField]
    private Camera minimapCam = null;
    [SerializeField]
    private Text timer = null, waveTimer = null, blueGold = null, qCD = null, wCD = null, eCD = null
        , rCD = null;
    [SerializeField]
    private GameObject hero = null;
    [SerializeField]
    private Button q = null, w = null, e = null, r = null, buttonHM = null;
    [SerializeField]
    private Text heroBeingAttackedText = null;
    public delegate void MiniMapInput(RaycastHit _hit);
    public event MiniMapInput GrabHit;
    private static readonly Color low = new Color(0.9f, 0.0f, 0.0f);
    private Image qIm, wIm, eIm, rIm;
    private HeroAbilities abilities;
    private RaycastHit hit;
    private ColorBlock BrightColor;
    private float tmr, buttonHmAnimTime = 0.0f, qtim, wtim, etim, rtim;
    private int tempSec;
    private void Start()
    {
        hero = GameManager.Instance.Player;
        if (!hero)
            hero = FindObjectOfType<HeroInfo>().gameObject;
        abilities = hero.GetComponent<HeroAbilities>();
        q.onClick.AddListener(abilities.GetAbilityQ.TryCast);
        w.onClick.AddListener(abilities.GetAbilityW.TryCast);
        e.onClick.AddListener(abilities.GetAbilityE.TryCast);
        r.onClick.AddListener(abilities.GetAbilityR.TryCast);
        qIm = q.image;
        wIm = w.image;
        eIm = e.image;
        rIm = r.image;
        BrightColor = ColorBlock.defaultColorBlock;
        BrightColor.colorMultiplier = 1.5f;
    }
    private void Update()
    {
        ButtonHMAnim();
        if (Input.GetMouseButton(1))
            if (Physics.Raycast(minimapCam.ScreenPointToRay(Input.mousePosition), out hit))
                GrabHit?.Invoke(hit);
        tmr = GameManager.Instance.Timer;
        if (tmr <= 0.0f)
            tmr = 0.0f;
        timer.text = Options.Japanese ? "秒読み：" : "Time left: ";
        if (tmr >= 60.0f)
            timer.text += System.Math.DivRem((int)tmr + 1, 60, out tempSec).ToString("D2") +
                ':' + tempSec.ToString("D2");
        else
            timer.text += tmr.ToString(tmr > 10.0f ? "F1" : "F2");
        if (Options.Japanese)
            waveTimer.text = "ウエーブ＃" + GameManager.Instance.Wave + " 次のウエーブ：";
        else
            waveTimer.text = "Wave #" + GameManager.Instance.Wave + " Next wave in: ";
        waveTimer.text += GameManager.Instance.WaveTimer;
        blueGold.text = (Options.Japanese ? "金 " : "Gold: ") + EconomyManager.Instance.BlueGold;
        Cooldowns();
    }
    private void ButtonHMAnim()
    {
        if (HeroUIScript.HeroBeingAttacked)
        {
            buttonHmAnimTime += Time.deltaTime;
            heroBeingAttackedText.enabled = true;
            if (0 == Mathf.CeilToInt(buttonHmAnimTime * 2.5f) % 2)
            {
                buttonHM.colors = BrightColor;
                heroBeingAttackedText.color = Color.red;
            }
            else
            {
                buttonHM.colors = ColorBlock.defaultColorBlock;
                heroBeingAttackedText.color = low;
            }
        }
        else
        {
            heroBeingAttackedText.enabled = false;
            buttonHM.colors = ColorBlock.defaultColorBlock;
        }
    }
    private void Cooldowns()
    {
        if (abilities)
        {
            qtim = abilities.GetAbilityQ.Timer;
            wtim = abilities.GetAbilityW.Timer;
            etim = abilities.GetAbilityE.Timer;
            rtim = abilities.GetAbilityR.Timer;
            qCD.text = qtim <= 0.0f ? "" : qtim.ToString("F2");
            wCD.text = wtim <= 0.0f ? "" : wtim.ToString("F2");
            eCD.text = etim <= 0.0f ? "" : etim.ToString("F2");
            rCD.text = rtim <= 0.0f ? "" : rtim.ToString("F2");
            q.interactable = abilities.GetAbilityQ.EnoughMana && qtim <= 0.0f;
            w.interactable = abilities.GetAbilityW.EnoughMana && wtim <= 0.0f;
            e.interactable = abilities.GetAbilityE.EnoughMana && etim <= 0.0f;
            r.interactable = abilities.GetAbilityR.EnoughMana && rtim <= 0.0f;
            if (GameManager.GameRunning)
                qIm.color = wIm.color = eIm.color = rIm.color = Color.white;
            else
            {
                if (!q.interactable)
                    qIm.color = q.colors.disabledColor;
                if (!w.interactable)
                    wIm.color = w.colors.disabledColor;
                if (!e.interactable)
                    eIm.color = e.colors.disabledColor;
                if (!r.interactable)
                    rIm.color = r.colors.disabledColor;
            }
        }
    }
}