﻿using UnityEngine;
using UnityEngine.UI;
public enum CamTransitionState { OnMain, Main2Hero, OnHero, Hero2Main }
public class HeroCamScript : MonoBehaviour
{
    [SerializeField]
    private Transform mainCameraTransform = null, heroTransform = null, heroTransformMax = null,
        heroCenter = null;
    [SerializeField]
    private KeyCode switchViewKey = KeyCode.C;
    [SerializeField]
    private float transitionSpeed = 1.5f;
    [SerializeField]
    private HeroInfo info = null;
    [SerializeField]
    private GameObject switchButton = null;
    [SerializeField]
    private Text hudTextAbilityQ = null, hudTextAbilityW = null, hudTextAbilityE = null,
        hudTextAbilityR = null;
    public delegate void voidDel();
    public static event voidDel OnOnHero;
    public static HeroCamScript inst;
    private static string enterHM, exitHM;
    private RaycastHit[] hits;
    private Camera mainCam, heroCam;
    private Button buttonSwitch;
    private Text textSwitch;
    private Color green, red;
    private CamTransitionState state = CamTransitionState.OnMain;
    private string hitLayerName;
    private float verticalRotation = 0.0f, maxVert = 55.0f, minVert = -30.0f, tValue = 0.0f,
        mouseVerticalStart, targetFOV = 60.0f, maxDistance, forceDistance, currentVertical, newVert,
        vertChange;
    private bool cameraReady = false;
    public static Camera HeroCam
    {
        get
        {
            if (inst)
                return inst.heroCam;
            return null;
        }
    }
    public static float MouseVertical
    {
        get { return inst ? inst.mouseVerticalStart : 0.0f; }
        set
        {
            if (inst)
                inst.mouseVerticalStart = value;
        }
    }
    public static bool IsOnVantage
    { get { return CameraControl.Vantage.enabled; } }
    public static bool IsOnHero
    {
        get
        {
            if (inst)
                return CamTransitionState.OnHero == inst.state;
            else
                return false;
        }
    }
    public static bool IsHeroAlive
    {
        get
        {
            if (!inst)
                return false;
            if (!inst.info)
                return false;
            return inst.info.Alive;
        }
    }
    private bool HudTextShowsQWER
    {
        set
        {
            if (value)
            {
                hudTextAbilityQ.text = "Q";
                hudTextAbilityW.text = "W";
                hudTextAbilityE.text = "E";
                hudTextAbilityR.text = "R";
            }
            else
            {
                hudTextAbilityQ.text = "1";
                hudTextAbilityW.text = "2";
                hudTextAbilityE.text = "3";
                hudTextAbilityR.text = "4";
            }
        }
    }
    public void SwitchView()
    {
        if (!info)
            return;
        switch (state)
        {
            case CamTransitionState.OnMain:
                if (!info.Alive)
                    return;
                heroCam.fieldOfView = mainCam.fieldOfView;
                transform.position = mainCameraTransform.position;
                transform.rotation = mainCameraTransform.rotation;
                heroCam.enabled = true;
                mainCam.enabled = false;
                state = CamTransitionState.Main2Hero;
                break;
            case CamTransitionState.Main2Hero:
                state = CamTransitionState.Hero2Main;
                break;
            case CamTransitionState.OnHero:
                state = CamTransitionState.Hero2Main;
                HudTextShowsQWER = true;
                break;
            case CamTransitionState.Hero2Main:
                if (!info.Alive)
                    return;
                state = CamTransitionState.Main2Hero;
                break;
            default:
                break;
        }
        AudioManager.PlaySoundEffect(AudioManager.sfxHeroCam);
    }
    private void Start()
    {
        mainCam = mainCameraTransform.gameObject.GetComponent<Camera>();
        heroCam = GetComponent<Camera>();
        inst = this;
        buttonSwitch = switchButton.GetComponent<Button>();
        textSwitch = switchButton.GetComponentInChildren<Text>();
        buttonSwitch.onClick.AddListener(SwitchView);
        green = new Color(0.49f, 0.6f, 0.0f);
        red = new Color(0.9f, 0.14f, 0.0f);
        mouseVerticalStart = -0.5f * Input.mousePosition.y;
        Options.OnChangedLanguage += SetHMStrings;
        SetHMStrings();
    }
    private void Update()
    {
        Cursor.visible = !GameManager.GameRunning || !IsOnHero || StateID.STATE_SHOP ==
            ApplicationManager.Instance.GetAppState() || heroCamDisabler.DisabledCameraMovement;
        if (!cameraReady)
        {
            GetHeroInfo();
            if (info)
                cameraReady = true;
            else
                return;
        }
        MouseVerticalAxis();
        switchButton.SetActive(info.Alive);
        CalcHeroCamPosition();
        if (Input.GetKeyDown(switchViewKey) && info.Alive)
            AudioManager.PlaySoundEffect(AudioManager.sfxHeroCam);
        switch (state)
        {
            case CamTransitionState.OnMain:
                textSwitch.text = enterHM;
                textSwitch.color = green;
                CamOnMain();
                break;
            case CamTransitionState.Main2Hero:
                textSwitch.text = exitHM;
                textSwitch.color = red;
                CamMain2Hero();
                break;
            case CamTransitionState.OnHero:
                textSwitch.text = exitHM;
                textSwitch.color = red;
                CamOnHero();
                break;
            case CamTransitionState.Hero2Main:
                textSwitch.text = enterHM;
                textSwitch.color = green;
                CamHero2Main();
                break;
            default:
                break;
        }
    }
    private void OnDestroy()
    {
        Options.OnChangedLanguage -= SetHMStrings;
    }
    private void SetHMStrings()
    {
        if (Options.Japanese)
            enterHM = exitHM = "(C) ヒーロ モード";
        else
        {
            enterHM = "(C) Enter Hero Mode";
            exitHM = "(C) Exit Hero Mode";
        }
    }
    private void CalcHeroCamPosition()
    {
        maxDistance = Vector3.Distance(heroCenter.position, heroTransformMax.position);
        hits = Physics.RaycastAll(heroCenter.position, heroTransformMax.position - heroCenter.
            position, maxDistance);
        forceDistance = maxDistance;
        foreach (RaycastHit hit in hits)
        {
            hitLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            if ("Terrain" == hitLayerName || "Trees and Rocks" == hitLayerName)
                if (hit.distance <= forceDistance)
                    forceDistance = hit.distance;
        }
        heroTransform.position = (heroTransformMax.position - heroCenter.position) * 0.9f * (
            forceDistance / maxDistance) + heroCenter.position;
    }
    private void MouseVerticalAxis()
    {
        if (GameManager.Tutorial)
            if (heroCamDisabler.DisabledCameraMovement)
                return;
        if (Input.GetMouseButton(2) && IsOnHero)
        {
            currentVertical = Input.mousePosition.y;
            newVert = Mathf.Clamp(verticalRotation + mouseVerticalStart - currentVertical,
                minVert, maxVert);
            vertChange = newVert - verticalRotation;
            verticalRotation = newVert;
            heroCenter.Rotate(heroCenter.right, vertChange, Space.World);
        }
    }
    private void GetHeroInfo()
    {
        if (GameManager.Instance.Player)
        {
            info = GameManager.Instance.Player.GetComponent<HeroInfo>();
            if (info)
            {
                heroCenter = info.heroCenter;
                heroTransformMax = new GameObject("MaxTransf").transform;
                heroTransformMax.SetParent(heroCenter);
                heroTransform = new GameObject("HeroCamTransform").transform;
                heroTransform.SetParent(heroCenter);
                heroTransformMax.localScale = heroTransform.localScale = Vector3.zero;
                if (info.thirdPerson)
                {
                    heroTransformMax.rotation = heroTransform.rotation = info.thirdPerson.rotation;
                    heroTransformMax.position = heroTransform.position = info.thirdPerson.position;
                }
                else
                {
                    heroTransformMax.localRotation = heroTransform.localRotation = Quaternion.Euler(
                        12.6756f, 0.0f, 0.0f);
                    heroTransformMax.localPosition = heroTransform.localPosition = new Vector3(0.0f
                        , 3.143125f, -7.536876f);
                }
            }
        }
    }
    private void CamOnMain()
    {
        if (Input.GetKeyDown(switchViewKey) && info.Alive)
        {
            heroCam.fieldOfView = mainCam.fieldOfView;
            transform.position = mainCameraTransform.position;
            transform.rotation = mainCameraTransform.rotation;
            heroCam.enabled = true;
            mainCam.enabled = false;
            state = CamTransitionState.Main2Hero;
        }
    }
    private void CamMain2Hero()
    {
        if (!info.Alive)
        {
            AudioManager.PlaySoundEffect(AudioManager.sfxHeroCam);
            state = CamTransitionState.Hero2Main;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        if (Input.GetKeyDown(switchViewKey))
            state = CamTransitionState.Hero2Main;
        else
        {
            tValue += transitionSpeed * Time.deltaTime;
            if (tValue >= 1.0f)
            {
                tValue = 1.0f;
                state = CamTransitionState.OnHero;
                OnOnHero?.Invoke();
            }
            transform.rotation = Quaternion.Slerp(mainCameraTransform.rotation, heroTransform.
                rotation, tValue);
            transform.position = Vector3.Lerp(mainCameraTransform.position, heroTransform.position,
                tValue);
            heroCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, tValue);
        }
    }
    private void CamOnHero()
    {
        if (info.Alive)
            if (Input.GetKeyDown(switchViewKey))
            {
                state = CamTransitionState.Hero2Main;
                HudTextShowsQWER = true;
            }
            else
            {
                HudTextShowsQWER = false;
                heroCam.transform.position = heroTransform.position;
                heroCam.transform.rotation = heroTransform.rotation;
            }
        else
        {
            HudTextShowsQWER = true;
            AudioManager.PlaySoundEffect(AudioManager.sfxHeroCam);
            state = CamTransitionState.Hero2Main;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void CamHero2Main()
    {
        if (Input.GetKeyDown(switchViewKey))
            state = CamTransitionState.Main2Hero;
        else
        {
            tValue -= transitionSpeed * Time.deltaTime;
            if (tValue <= 0.0f)
            {
                tValue = 0.0f;
                mainCam.enabled = true;
                heroCam.enabled = false;
                state = CamTransitionState.OnMain;
            }
            transform.rotation = Quaternion.Slerp(mainCameraTransform.rotation, heroTransform.
                rotation, tValue);
            transform.position = Vector3.Lerp(mainCameraTransform.position, heroTransform.position,
                tValue);
            heroCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, tValue);
        }
    }
}