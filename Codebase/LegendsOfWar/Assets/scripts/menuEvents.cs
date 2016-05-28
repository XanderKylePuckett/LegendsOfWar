﻿using UnityEngine;
public class menuEvents : MonoBehaviour
{
	[SerializeField]
	private GameObject loading = null;

	public void ChangeAppState( string appState )
	{
		Loading();
		ApplicationManager.Instance.ChangeAppState( appState );
	}
	public void PlayClickSound()
	{
		AudioManager.PlayClickSound();
	}
	public void GoBack()
	{
		ApplicationManager.ReturnToPreviousState();
	}
	public void Loading()
	{
		if ( loading )
			loading.SetActive( true );
	}
}