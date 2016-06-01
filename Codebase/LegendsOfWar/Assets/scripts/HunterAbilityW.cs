﻿using UnityEngine;
public class HunterAbilityW : AbilityWBase
{
	[SerializeField]
	private float range = 0.0f, speed = 0.0f, damage = 0.0f;
	[SerializeField]
	private GameObject projectile = null, arrowSpawn = null, Icon = null;
	private GameObject activeIcon = null;
	private SpriteRenderer activeIconSpriteRenderer = null;
	private NavMeshAgent targNm = null, colNm;
	private Info target = null, colInfo;
	private RaycastHit hit;
	private float originalSpeed = 0.0f;
	protected override void Update()
	{
		skillTimer -= Time.deltaTime;
		if ( abilityOn && skillTimer <= 0.0f )
			AbilityDeactivate();
		if ( EnoughMana )
			if ( ( ( Input.GetKeyDown( KeyCode.W ) && !HeroCamScript.onHero ) || Input.GetKeyDown(
				KeyCode.Alpha2 ) || Input.GetKeyDown( KeyCode.Keypad2 ) ) && cooldownTimer <= 0.0f )
				if ( TargetSelected() )
					TryCast();
		if ( !target && activeIcon && activeIconSpriteRenderer.enabled )
			activeIconSpriteRenderer.enabled = false;
	}
	protected override void AbilityActivate()
	{
		base.AbilityActivate();
		originalSpeed = targNm.speed;
		targNm.speed = 0.0f;
		activeIcon = Instantiate( Icon, target.transform.position, target.transform.rotation ) as
			GameObject;
		activeIconSpriteRenderer = activeIcon.GetComponent<SpriteRenderer>();
		activeIcon.transform.parent = target.transform;
		activeIconSpriteRenderer.enabled = true;
		ProjectileBehaviour p = ( Instantiate( projectile, arrowSpawn.transform.position, arrowSpawn
			.transform.rotation ) as GameObject ).GetComponent<ProjectileBehaviour>();
		p.speed = speed;
		p.damage = damage;
		p.target = hit.transform;
		p.Fire();
	}
	protected override void AbilityDeactivate()
	{
		base.AbilityDeactivate();
		if ( target )
			targNm.speed = originalSpeed;
		if ( activeIconSpriteRenderer )
			activeIconSpriteRenderer.enabled = false;
	}
	private bool TargetSelected()
	{
		Ray ray = new Ray( transform.position, transform.forward );
		if ( Physics.SphereCast( ray, 5.0f, out hit, range, 9, QueryTriggerInteraction.Collide ) )
		{
			colInfo = hit.collider.GetComponent<Info>();
			colNm = hit.collider.GetComponent<NavMeshAgent>();
			if ( colNm && colInfo && colInfo.team != heroInfo.team )
			{
				target = colInfo;
				targNm = colNm;
				return true;
			}
		}
		return false;
	}
}
#region OLD_CODE
#if false
using UnityEngine;

public class HunterAbilityW : AbilityWBase
{
    //Rooting Shot
    [SerializeField]
    float range = 0.0f, speed = 0.0f, damage = 0.0f;
    [SerializeField]
    GameObject projectile = null, arrowSpawn = null;

    [SerializeField]
    GameObject Icon = null;

    GameObject activeIcon = null;

    Info target = null;
    float originalSpeed = 0.0f;
    RaycastHit hit;

    //void Awake()
    //{
    //    m_effect = new Effect();
    //    m_effect.Name = "Rooting shot";
    //    m_effect.Type = StatusEffectType.SNARE;
    //    m_effect.Duration = 3;
    //}

    protected override void Update()
    {
        skillTimer -= Time.deltaTime;
        if (abilityOn && skillTimer <= 0.0f)
            AbilityDeactivate();
		// <BUGFIX: Test Team #32>
		if(EnoughMana)
		// </BUGFIX: Test Team #32>
        if (((Input.GetKeyDown(KeyCode.W) && !HeroCamScript.onHero) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2)) && cooldownTimer <= 0.0f)
        {
            if (TargetSelected())
                TryCast();
        }

        if (!target && activeIcon && activeIcon.GetComponent<SpriteRenderer>().enabled)
            activeIcon.GetComponent<SpriteRenderer>().enabled = false;
    }

    protected override void AbilityActivate()
    {
        base.AbilityActivate();
        StopTarget();
    }

    MarkedEnemyIcon mei;
    protected override void AbilityDeactivate()
    {
        base.AbilityDeactivate();

        if (target)
            target.GetComponent<NavMeshAgent>().speed = originalSpeed;

        if (activeIcon && activeIcon.GetComponent<SpriteRenderer>())
            activeIcon.GetComponent<SpriteRenderer>().enabled = false;
    }

    bool TargetSelected()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(ray, 5.0f, out hit, range, 9, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.GetComponent<NavMeshAgent>() && hit.collider.GetComponent<Info>() && hit.collider.GetComponent<Info>().team != GetComponentInParent<Info>().team)
            {
                target = hit.collider.gameObject.GetComponent<Info>();
                return true;
            }
        }
        return false;
    }

    void StopTarget()
    {
        originalSpeed = target.GetComponent<NavMeshAgent>().speed;
        target.GetComponent<NavMeshAgent>().speed = 0.0f;
        //StatusEffects.Inflict(target.gameObject, Effect.CreateEffect());
        activeIcon = (Instantiate(Icon, target.transform.position, target.transform.rotation) as GameObject);
        activeIcon.transform.parent = target.transform;
        activeIcon.GetComponent<SpriteRenderer>().enabled = true;
        ProjectileBehaviour p = (Instantiate(projectile, arrowSpawn.transform.position, arrowSpawn.transform.rotation) as GameObject).GetComponent<ProjectileBehaviour>();
        p.speed = speed;
        p.damage = damage;
        p.target = hit.transform;
        p.Fire();
    }
}
#endif
#endregion //OLD_CODE