﻿using UnityEngine;
public enum MinionClass { STRIKER_MINION, TANK_MINION, CASTER_MINION, SIEGE_MINION }
public class MinionInfo : Info
{
	public MinionClass type;
	[SerializeField]
	private int movementSpeed = 0;
	private bool m_soulDefense = false;
	private float baseDamage = 0.0f;
	public bool soulDefense
	{
		get { return m_soulDefense; }
		set { m_soulDefense = value; }
	}
	public int MovementSpeed
	{ get { return movementSpeed; } }
	public float Damage
	{
		get { return damage; }
		set { damage = value; }
	}
	public float Range
	{
		get { return attackRange; }
		set { attackRange = value; }
	}
	public float AttackSpeed
	{
		get { return attackSpeed; }
		set { attackSpeed = value; }
	}
	public float AgroRange
	{ get { return agroRange; } }
	public override void TakeDamage( float damage )
	{
		if ( m_soulDefense )
			base.TakeDamage( damage * 0.5f );
		else
			base.TakeDamage( damage );
	}
	public void Cyclone()
	{
		baseDamage *= 0.75f;
	}
	protected override void Start()
	{
		base.Start();
		Attacked += MinionAttacked;
		Destroyed += MinionDeath;
		baseDamage = damage;
	}
	private void Update()
	{
		if ( SupportRange.InSupportRange( gameObject ) )
			damage = baseDamage * 1.5f;
		else
			damage = baseDamage;
	}
	private void MinionAttacked()
	{
		AudioManager.PlaySoundEffect( AudioManager.sfxMinionAttacked, transform.position );
	}
	private void MinionDeath()
	{
		AudioManager.PlaySoundEffect( AudioManager.sfxMinionDeath, transform.position );
	}
}