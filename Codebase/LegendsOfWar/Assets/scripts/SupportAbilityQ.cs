﻿using UnityEngine;
public class SupportAbilityQ : AbilityQBase
{
	private ParticleSystem ps;
	private SupportRange supprang;
	protected override void Start()
	{
		base.Start();
		ps = GetComponentInChildren<ParticleSystem>();
		supprang = heroInfo.GetComponentInChildren<SupportRange>();
	}
	protected override void AbilityActivate()
	{
		base.AbilityActivate();
		supprang.ApplyToAlliesInRange( SoothingAura );
		ps.Play();
	}
	protected override void AbilityDeactivate()
	{
		base.AbilityDeactivate();
		ps.Stop();
		ps.Clear();
	}
	private void SoothingAura( Info entity )
	{
		if ( entity is MinionInfo )
			entity.HP += 10.0f;
	}
}
#region OLD_CODE
#if false
#endif
#endregion //OLD_CODE