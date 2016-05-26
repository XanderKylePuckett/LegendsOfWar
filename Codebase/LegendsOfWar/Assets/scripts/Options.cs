﻿using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	public delegate void optionsChangedEvent();
	public static event optionsChangedEvent onChangedLanguage;
	public static event optionsChangedEvent onChangedBgmVolume;
	public static event optionsChangedEvent onChangedSfxVolume;
	public static event optionsChangedEvent onChangedVoiceVolume;

	public static bool IsAdditive { get; set; }
	public static SystemLanguage applicationLanguage { get; private set; }
	public static float bgmVolume { get; private set; }
	public static float sfxVolume { get; private set; }
	public static float voiceVolume { get; private set; }
	private static string language { get; set; }
	public static bool Japanese { get { return SystemLanguage.Japanese == applicationLanguage; } }
	public static void Init()
	{
		IsAdditive = false;
		applicationLanguage = SystemLanguage.English;
		bgmVolume = 0.25f;
		sfxVolume = 0.8f;
		voiceVolume = 1.0f;
		language = "English";

		bgmVolume = PlayerPrefs.GetFloat( "MusicVolume", bgmVolume );
		PlayerPrefs.SetFloat( "MusicVolume", bgmVolume );
		if ( onChangedBgmVolume != null )
			onChangedBgmVolume();

		sfxVolume = PlayerPrefs.GetFloat( "SfxVolume", sfxVolume );
		PlayerPrefs.SetFloat( "SfxVolume", sfxVolume );
		if ( onChangedSfxVolume != null )
			onChangedSfxVolume();

		voiceVolume = PlayerPrefs.GetFloat( "VoiceVolume", voiceVolume );
		PlayerPrefs.SetFloat( "VoiceVolume", voiceVolume );
		if ( onChangedVoiceVolume != null )
			onChangedVoiceVolume();

		language = PlayerPrefs.GetString( "Language", language );
		PlayerPrefs.SetString( "Language", language );
		applicationLanguage = "Japanese" == language ?
			SystemLanguage.Japanese : SystemLanguage.English;
		if ( onChangedLanguage != null )
			onChangedLanguage();
	}

	[SerializeField]
	Slider bgmSlider = null, sfxSlider = null, voiceSlider = null;
	[SerializeField]
	GameObject menuCam = null;
	//[SerializeField]
	//Image bgPanel = null;

	//Color mainColor = new Color( 0.196078435f, 0.0f, 0.196078435f, 0.3529412f ),
	//	ingameColor = new Color( 0.196078435f, 0.0f, 0.196078435f, 0.784313738f );

	void Awake()
	{
		if ( IsAdditive )
		{
			menuCam.SetActive( false );
			//bgPanel.color = ingameColor;
		}
		//else
		//	bgPanel.color = mainColor;
	}
	void Start()
	{
		bgmSlider.normalizedValue = bgmVolume;
		sfxSlider.normalizedValue = sfxVolume;
		voiceSlider.normalizedValue = voiceVolume;
	}
	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
			ApplicationManager.ReturnToPreviousState();
	}
	public void PlayTestSound()
	{
		AudioManager.PlaySoundEffect( AudioManager.sfxTowerAttacked );
	}
	[SerializeField]
	AudioClip voice;
	public void PlayVoiceSound()
	{
		AudioManager.PlayClipRaw( voice, null, true );
	}
	public void BgmVolumeChanging()
	{
		bgmVolume = bgmSlider.normalizedValue;
		PlayerPrefs.SetFloat( "MusicVolume", bgmVolume );
		if ( onChangedBgmVolume != null )
			onChangedBgmVolume();
	}
	public void SfxVolumeChanging()
	{
		sfxVolume = sfxSlider.normalizedValue;
		PlayerPrefs.SetFloat( "SfxVolume", sfxVolume );
		if ( onChangedSfxVolume != null )
			onChangedSfxVolume();
	}
	public void VoiceVolumeChanging()
	{
		voiceVolume = voiceSlider.normalizedValue;
		PlayerPrefs.SetFloat( "VoiceVolume", voiceVolume );
		if ( onChangedVoiceVolume != null )
			onChangedVoiceVolume();
	}
	public void toggleLanguage()
	{
		toggleLanguage_Static();
	}
	public static void toggleLanguage_Static()
	{
		switch ( applicationLanguage )
		{
			case SystemLanguage.English:
				applicationLanguage = SystemLanguage.Japanese;
				break;
			default:
				applicationLanguage = SystemLanguage.English;
				break;
		}

		PlayerPrefs.SetString( "Language", Japanese ? "Japanese" : "English" );

		if ( onChangedLanguage != null )
			onChangedLanguage();
	}


}