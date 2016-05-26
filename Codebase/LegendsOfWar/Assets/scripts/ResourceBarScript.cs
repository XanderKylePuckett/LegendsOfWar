﻿using UnityEngine;
using UnityEngine.UI;

public class ResourceBarScript : MonoBehaviour
{
	[SerializeField]
	GameObject host = null;

	Info stats;
	Image bar = null;

	[SerializeField]
	bool attachedToHUD = false;

	public bool isMana = false;

	void Start()
	{
		stats = host.GetComponent<Info>();
		bar = GetComponent<Image>();
		bar.type = Image.Type.Filled;
		bar.fillMethod = Image.FillMethod.Horizontal;
		bar.fillOrigin = 0;
		high = transform.localPosition;
		low = high * 0.6f;
		notHero = !GetComponentInParent<HeroInfo>();
		heroUiTrans = HeroUIScript.Instance.transform;
		rectTransform = GetComponent<RectTransform>();
	}

	bool notHero = true;
	Vector3 high, low;
	static readonly Quaternion x90 = Quaternion.Euler( 90.0f, 0.0f, 0.0f );
	Transform heroUiTrans;
	RectTransform rectTransform;
	void Update()
	{
		if ( isMana )
			bar.fillAmount = ( ( stats as HeroInfo ).Mana / ( stats as HeroInfo ).MaxMana );
		else
			bar.fillAmount = stats.HP / stats.MAXHP;

		if ( !attachedToHUD )
		{
			if ( HeroCamScript.onHero && notHero )
			{
				transform.localPosition = low;
				transform.LookAt( 2.0f * transform.position - heroUiTrans.position, heroUiTrans.up );
				gameObject.layer = 11;
			}
			else
			{
				transform.localPosition = high;
				transform.localRotation = x90;
				rectTransform.Rotate( Vector3.up, -transform.rotation.eulerAngles.y, Space.World );
				gameObject.layer = 5;
			}
		}
	}

	public GameObject Host { set { host = value; } get { return host; } }
}