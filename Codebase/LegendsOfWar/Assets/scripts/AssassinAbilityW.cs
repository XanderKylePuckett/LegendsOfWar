﻿using UnityEngine;
public class AssassinAbilityW : AbilityWBase
{
	[SerializeField]
	private GameObject Target = null;
	[SerializeField]
	private int Damage = 0, Speed = 0;
	[SerializeField]
	protected Detector attackTrigger;
	[SerializeField]
	protected GameObject weapon, projectile;
	public GameObject[ ] Marked;
	private static int MarkNum = 0;

	protected override void AbilityActivate()
	{
		base.AbilityActivate();
		FireAtTarget( Target.transform, Speed, Damage );
	}
	protected void FireAtTarget( Transform _target, float _speed, float _damage )
	{
		if ( _target )
		{
			SkillShot p = ( Instantiate( projectile, weapon.transform.position, weapon.transform.
				rotation ) as GameObject ).GetComponent<SkillShot>();
			p.MarkingAttack = true;
			p.speed = _speed;
			p.damage = _damage;
			p.target = _target;
			p.Shooter = weapon;
			p.effect = m_effect.CreateEffect();
			p.Fire();
		}
	}
	public bool MarkHit( GameObject _mark )
	{
		if ( MarkNum <= 3 )
		{
			if ( MarkNum >= 1 )
			{
				if ( Marked[ MarkNum - 1 ] != _mark )
					MarkNum = 0;
				if ( MarkNum == 3 )
					MarkNum = 0;
			}
			Marked[ MarkNum++ ] = _mark;
			return true;
		}
		return false;
	}
}