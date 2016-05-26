﻿using UnityEngine;
using System.Collections;

public class CasterQ : AbilityQBase
{
	[SerializeField]
	GameObject m_Engulf = null;
	[SerializeField]
	GameObject m_targetingSystem = null;
	ParticleSystem m_targetingEffect = null;
	RaycastHit m_targetHit;


	protected override void Start()
	{
		base.Start();
		m_Engulf.GetComponent<ParticleSystem>().Stop();
		m_Engulf.GetComponent<ParticleSystem>().Clear();
		m_targetingEffect = m_targetingSystem.GetComponent<ParticleSystem>();
		m_targetingEffect.Stop();
		m_targetingEffect.Clear();
		cooldownTime = 0.8f;
	}
	void FixedUpdate()
	{
		if ( aimingSkill )
		{
			if ( Physics.SphereCast( transform.parent.position, 5.0f, transform.forward, out m_targetHit, 150.0f, 1 ) )
			{
				if ( m_targetHit.collider.gameObject.tag == "Minion" && m_targetHit.collider.gameObject.GetComponentInParent<Info>().team == Team.RED_TEAM )
				{
					m_targetingSystem.transform.position = m_targetHit.collider.gameObject.transform.position;
					m_targetingEffect.Play();
				}
				else if ( m_targetingEffect.isPlaying )
					ResetSystem();
			}
			else if ( m_targetingEffect.isPlaying )
				ResetSystem();
		}
		else if ( m_targetingEffect.isPlaying )
			ResetSystem();

	}

	void ResetSystem()
	{
		if ( m_targetingEffect.isPlaying )
			m_targetingEffect.Stop();
		m_targetingEffect.Clear();
		m_targetingSystem.transform.localPosition = Vector3.zero;
	}

	protected override void Update()
	{

		if ( Input.GetMouseButtonDown( 0 ) && aimingSkill )
		{
			aimingSkill = false;
			//Input.ResetInputAxes();
			TryCast();
			ResetSystem();
		}
		else if ( Input.GetMouseButtonDown( 1 ) && aimingSkill )
		{
			aimingSkill = false;
		}
		else if ( ( ( Input.GetKeyDown( KeyCode.Q ) && !HeroCamScript.onHero ) ||
					Input.GetKeyDown( KeyCode.Alpha1 ) ||
					Input.GetKeyDown( KeyCode.Keypad1 ) ) && !aimingSkill && cooldownTimer <= 0.0f )
			aimingSkill = true;
		// <BUGFIX: Test Team #32>
		if ( !EnoughMana )
			aimingSkill = false;
		// </BUGFIX: Test Team #32>
	}

	protected override void AbilityActivate()
	{
		if ( m_targetHit.collider != null )
		{
			GameObject tmp = Instantiate( m_Engulf, m_targetHit.transform.position, m_targetHit.transform.rotation ) as GameObject;
			tmp.GetComponent<CasterEBehavior>().Activate = true;
			tmp.transform.parent = m_targetHit.transform;
			StatusEffects.Inflict( m_targetHit.collider.gameObject, Effect.CreateEffect() );
			m_targetHit.collider.gameObject.GetComponentInParent<Info>().TakeDamage( m_effect.m_damage );
			base.AbilityActivate();
		}
	}
}
