﻿using UnityEngine;
using System.Collections.Generic;
public class TankAbilityE : AbilityEBase
{
	public CollisionDetector coll;
	public float Edamage;
	private List<Info> slowed;
	private GameObject AbilityEParticle;
	protected override void Start()
	{
		base.Start();
		AbilityEParticle = GameObject.FindGameObjectWithTag( "PE" );
		AbilityEParticle.GetComponent<ParticleSystem>().Stop();
		slowed = new List<Info>();
	}
	protected override void AbilityActivate()
	{
		base.AbilityActivate();
		AbilityEParticle.GetComponent<Transform>().localPosition = new Vector3( GameObject.
			FindGameObjectWithTag( "Hero" ).GetComponent<Transform>().localPosition.x, 1, GameObject
			.FindGameObjectWithTag( "Hero" ).GetComponent<Transform>().localPosition.z );
		AbilityEParticle.GetComponent<ParticleSystem>().Play();
		if ( coll )
			coll.DealDamage( Slow );
	}
	protected override void AbilityDeactivate()
	{
		base.AbilityDeactivate();
		if ( 0 != slowed.Count )
			for ( int i = 0; i < slowed.Count; ++i )
				if ( slowed[ i ] )
					slowed[ i ].gameObject.GetComponent<NavMeshAgent>().speed += 10.0f;
		slowed.Clear();
		AbilityEParticle.GetComponent<Transform>().localPosition -= new Vector3( 0.0f, 10.0f );
		AbilityEParticle.GetComponent<ParticleSystem>().Stop();
		AbilityEParticle.GetComponent<ParticleSystem>().Clear();
	}
	private void Slow( Info entity )
	{
		if ( entity )
			if ( entity is MinionInfo )
			{
				slowed.Add( entity );
				entity.TakeDamage( Edamage );
				entity.gameObject.GetComponent<NavMeshAgent>().speed -= 10.0f;
			}
	}
}