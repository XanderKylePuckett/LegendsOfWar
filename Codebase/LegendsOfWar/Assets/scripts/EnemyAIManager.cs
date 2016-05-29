﻿using UnityEngine;
using System.Collections.Generic;
public enum HeroLocation { TOP_Lane, MID_Lane, BOT_Lane, Close, TOO_Close, Unknown }
public enum DangerLevel { MINIMAL, LOW, MEDIUM, MODERATE, HIGH, CRITICAL, EXTREME }
public class EnemyAIManager : MonoBehaviour
{
	[Header( "Needed stuff" )]
	public PortalInfo redPortal = null;
	public PortalInfo bluePortal = null;
	public GameObject UpperSplit = null;
	public GameObject LowerSplit = null;
	public GameObject portalLazer = null;
	public BcWeapon lazer = null;
	public GameObject lastResortParticle = null;
	public Detector attackRange = null;
	[Header( "Logic variables" )]
	[Range( 0.0f, 100.0f )]
	public float dangerTreshold = 0.0f;
	[Range( 0, 6 )]
	public int towersRemaining = 6;
	[Range( 0.0f, 5000.0f )]
	public float remainingHealth = 0.0f;
	public HeroLocation heroPresence = HeroLocation.Unknown;
	[Header( "Timers" )]
	[Range( 0.0f, 80.0f )]
	public float siegeTimer = 0.0f;
	[Range( 0.0f, 80.0f )]
	public float reinforcementsTimer = 0.0f;
	[Range( 0.0f, 30.0f )]
	public float lastResortTimer = 3.0f;
#if DEBUG
	[Header( "Enabled Behaviors" )]
	public bool m_spawnSiegeMinion = false;
	public bool m_reinforcements = false;
	public bool m_towerMovement = false;
	public bool m_selfRecover = false;
	public bool m_huntHero = false;
	public bool m_upgradeSiege = false;
	public bool m_extraSiegeMinion = false;
	public bool m_lastResort = false;
#endif
	public static bool spawnSiegeMinion = false;
	public static bool reinforcements = false;
	public static bool towerMovement = false;
	public static bool selfRecover = false;
	public static bool huntHero = false;
	public static bool upgradeSiege = false;
	public static bool extraSiegeMinion = false;
	public static bool lastResort = false;
	[Header( "Minion Prefabs" )]
	public GameObject siegeMinion = null;
	public GameObject redStriker = null;
	public GameObject redTank = null;
	public GameObject redCaster = null;
#if DEBUG
	public float temp;
	public float temp2 = 0.0f;
#endif
	private static readonly Quaternion faceLeft = new Quaternion( 0.0f, -0.707106781f, 0.0f,
		0.707106781f ), faceUp = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f ), faceDown = new Quaternion
		( 0.0f, 1.0f, 0.0f, 0.0f );
	private float reinforcementsTime = 40.0f;
	private float siegeTime = 60.0f;
	private float lastResortTime = 20.0f;
	private bool LastResortActive = false;
	private float towerCost = 9.0f;
	private float healthCost = 50.0f;
	private float timeCost = 40.0f;
	private float selfRecoveryBase = 20.0f;
	private float selfRecoveryGrowth = 8.0f;
	private float maxTime = 900.0f;
	private GameObject min_go;
	private NavMeshAgent nma;
	private List<Transform> targets = new List<Transform>();
	private GameObject Hero = null;
#if DEBUG
	private float second = 1.0f;
#endif
	private float CalcSelfRecovery
	{
		get
		{
			return selfRecoveryBase + selfRecoveryGrowth * ( ( ( ( redPortal.MAXHP - redPortal.HP )
				/ redPortal.MAXHP ) * selfRecoveryBase ) + ( 0.0175f * ( maxTime - GameManager.
				Instance.Timer ) ) );
		}
	}
	public void SpawnTankMinion( Team team, int lane )
	{
		switch ( lane )
		{
			case 1:
				SetupMinion( Instantiate( redTank, redPortal.LeftSpawn[ Random.Range( 0, 5 ) ].
					position, faceUp ), Path.NORTH_PATH, Team.RED_TEAM );
				break;
			case 2:
				SetupMinion( Instantiate( redTank, redPortal.MidSpawn[ Random.Range( 0, 5 ) ].
					position, faceLeft ), Path.CENTER_PATH, Team.RED_TEAM );
				break;
			case 3:
				SetupMinion( Instantiate( redTank, redPortal.RightSpawn[ Random.Range( 0, 5 ) ].
					position, faceDown ), Path.SOUTH_PATH, Team.RED_TEAM );
				break;
			default:
				return;
		}
	}
	public void SpawnCasterMinion( Team team, int lane )
	{
		switch ( lane )
		{
			case 1:
				SetupMinion( Instantiate( redCaster, redPortal.LeftSpawn[ Random.Range( 0, 5 ) ].
					position, faceUp ), Path.NORTH_PATH, Team.RED_TEAM );
				break;
			case 2:
				SetupMinion( Instantiate( redCaster, redPortal.MidSpawn[ Random.Range( 0, 5 ) ].
					position, faceLeft ), Path.CENTER_PATH, Team.RED_TEAM );
				break;
			case 3:
				SetupMinion( Instantiate( redCaster, redPortal.RightSpawn[ Random.Range( 0, 5 ) ].
					position, faceDown ), Path.SOUTH_PATH, Team.RED_TEAM );
				break;
			default:
				return;
		}
	}
	public void SpawnStrikerMinion( Team team, int lane )
	{
		switch ( lane )
		{
			case 1:
				SetupMinion( Instantiate( redStriker, redPortal.LeftSpawn[ Random.Range( 0, 5 ) ].
					position, faceUp ), Path.NORTH_PATH, Team.RED_TEAM );
				break;
			case 2:
				SetupMinion( Instantiate( redStriker, redPortal.MidSpawn[ Random.Range( 0, 5 ) ].
					position, faceLeft ), Path.CENTER_PATH, Team.RED_TEAM );
				break;
			case 3:
				SetupMinion( Instantiate( redStriker, redPortal.RightSpawn[ Random.Range( 0, 5 ) ].
					position, faceDown ), Path.SOUTH_PATH, Team.RED_TEAM );
				break;
			default:
				return;
		}
	}
	public void SpawnSiegeMinion( Team team, HeroLocation lane )
	{
		switch ( lane )
		{
			case HeroLocation.BOT_Lane:
				SetupMinion( Instantiate( siegeMinion, redPortal.LeftSpawn[ Random.Range( 0, 5 ) ].
					position, faceUp ), Path.SOUTH_PATH, Team.RED_TEAM );
				break;
			case HeroLocation.TOP_Lane:
				SetupMinion( Instantiate( siegeMinion, redPortal.RightSpawn[ Random.Range( 0, 5 ) ].
					position, faceDown ), Path.NORTH_PATH, Team.RED_TEAM );
				break;
			default:
				SetupMinion( Instantiate( siegeMinion, redPortal.MidSpawn[ Random.Range( 0, 5 ) ].
					position, faceLeft ), Path.CENTER_PATH, Team.RED_TEAM );
				return;
		}
	}
	public void Destroyed()
	{
		--towersRemaining;
	}
	private void Start()
	{
		remainingHealth = redPortal.HP;
		Hero = GameManager.Instance.Player;
		attackRange.triggerEnter += AttackRange_triggerEnter;
		attackRange.triggerExit += AttackRange_triggerExit;
		attackRange.CreateTrigger( 210.0f );
	}
	private void Update()
	{
#if DEBUG
		m_spawnSiegeMinion = spawnSiegeMinion;
		m_reinforcements = reinforcements;
		m_towerMovement = towerMovement;
		m_selfRecover = selfRecover;
		m_huntHero = huntHero;
		m_upgradeSiege = upgradeSiege;
		m_extraSiegeMinion = extraSiegeMinion;
		m_lastResort = lastResort;
#endif
		if ( !Hero.activeSelf )
			lazer.autofire = false;
		remainingHealth = redPortal.HP;
		dangerTreshold = ( ( timeCost - ( ( GameManager.Instance.Timer / maxTime ) * timeCost ) ) +
			( healthCost - ( ( redPortal.HP / redPortal.MAXHP ) * healthCost ) ) + ( 5 -
			towersRemaining ) * towerCost );
		lazer.bulletPrefab.GetComponent<SiegeProjectile>().damage = GetComponentInParent<PortalInfo>
			().Damage * towersRemaining;
		if ( !LastResortActive )
			switch ( GetTriggered( dangerTreshold ) )
			{
				case DangerLevel.EXTREME:
					selfRecover = true;
					spawnSiegeMinion = true;
					reinforcements = true;
					towerMovement = true;
					huntHero = true;
					upgradeSiege = true;
					extraSiegeMinion = true;
					lastResort = true;
					reinforcementsTime = 10.0f;
					siegeTime = 10.0f;
					selfRecoveryBase = 30.0f;
					break;
				case DangerLevel.CRITICAL:
					selfRecover = true;
					spawnSiegeMinion = true;
					reinforcements = true;
					towerMovement = true;
					huntHero = true;
					upgradeSiege = true;
					extraSiegeMinion = true;
					lastResort = false;
					reinforcementsTime = 20.0f;
					siegeTime = 15.0f;
					selfRecoveryBase = 20.0f;
					break;
				case DangerLevel.HIGH:
					selfRecover = true;
					spawnSiegeMinion = true;
					reinforcements = true;
					towerMovement = true;
					huntHero = false;
					upgradeSiege = true;
					extraSiegeMinion = true;
					lastResort = false;
					reinforcementsTime = 40.0f;
					siegeTime = 25.0f;
					selfRecoveryBase = 15.0f;
					break;
				case DangerLevel.MODERATE:
					selfRecover = true;
					spawnSiegeMinion = true;
					reinforcements = true;
					towerMovement = true;
					huntHero = false;
					upgradeSiege = true;
					extraSiegeMinion = false;
					lastResort = false;
					reinforcementsTime = 60.0f;
					siegeTime = 40.0f;
					selfRecoveryBase = 10.0f;
					break;
				case DangerLevel.MEDIUM:
					selfRecover = false;
					spawnSiegeMinion = true;
					reinforcements = true;
					towerMovement = true;
					huntHero = false;
					upgradeSiege = false;
					extraSiegeMinion = false;
					lastResort = false;
					reinforcementsTime = 80.0f;
					siegeTime = 60.0f;
					break;
				case DangerLevel.LOW:
					selfRecover = false;
					spawnSiegeMinion = true;
					reinforcements = false;
					towerMovement = false;
					huntHero = false;
					upgradeSiege = false;
					extraSiegeMinion = false;
					lastResort = false;
					reinforcementsTime = 80.0f;
					siegeTime = 80.0f;
					break;
				case DangerLevel.MINIMAL:
					selfRecover = false;
					spawnSiegeMinion = false;
					reinforcements = false;
					towerMovement = false;
					huntHero = false;
					upgradeSiege = false;
					extraSiegeMinion = false;
					lastResort = false;
					reinforcementsTime = 80.0f;
					siegeTime = 80.0f;
					break;
				default:
					break;
			}
		if ( spawnSiegeMinion )
			siegeTimer -= Time.deltaTime;
		if ( reinforcements )
			reinforcementsTimer -= Time.deltaTime;
		if ( lastResort && lastResortTimer >= 15.0f )
			LastResortActive = true;
		if ( redPortal.Alive && selfRecover )
		{
			redPortal.HP = remainingHealth + CalcSelfRecovery * Time.deltaTime;
			if ( LastResortActive )
			{
				lastResortParticle.GetComponent<ParticleSystem>().Play();
				lastResortTimer -= Time.deltaTime;
				selfRecoveryBase *= 3.0f;
				redPortal.HP = remainingHealth + CalcSelfRecovery * Time.deltaTime;
				if ( lastResortTimer <= 0.0f )
					LastResortActive = false;
			}
			else
			{
				lastResortParticle.GetComponent<ParticleSystem>().Stop();
				lastResortParticle.GetComponent<ParticleSystem>().Clear();
				lastResortTimer = System.Math.Min( lastResortTime, ( lastResortTimer + 0.6f * Time.
					deltaTime ) );
			}
		}
		if ( spawnSiegeMinion && siegeTimer <= 0.0f )
		{
			SpawnSiegeMinion( Team.RED_TEAM, heroPresence );
			if ( extraSiegeMinion )
				SpawnSiegeMinion( Team.RED_TEAM, heroPresence );
			siegeTimer = siegeTime;
		}
		if ( reinforcements && reinforcementsTimer <= 0.0f )
		{
			SetupMinion( Instantiate( redCaster, redPortal.LeftSpawn[ 0 ].position, faceDown ), Path
				.SOUTH_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redStriker, redPortal.LeftSpawn[ 3 ].position, faceDown ),
				Path.SOUTH_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redTank, redPortal.LeftSpawn[ 1 ].position, faceDown ), Path.
				SOUTH_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redCaster, redPortal.MidSpawn[ 0 ].position, faceLeft ), Path.
				CENTER_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redStriker, redPortal.MidSpawn[ 4 ].position, faceLeft ), Path
				.CENTER_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redTank, redPortal.MidSpawn[ 2 ].position, faceLeft ), Path.
				CENTER_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redCaster, redPortal.RightSpawn[ 0 ].position, faceUp ), Path.
				NORTH_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redStriker, redPortal.RightSpawn[ 4 ].position, faceUp ), Path
				.NORTH_PATH, Team.RED_TEAM );
			SetupMinion( Instantiate( redTank, redPortal.RightSpawn[ 2 ].position, faceUp ), Path.
				NORTH_PATH, Team.RED_TEAM );
			reinforcementsTimer = reinforcementsTime;
		}
		if ( Vector3.Distance( Hero.transform.position, redPortal.gameObject.transform.position ) <=
			130.0f )
			heroPresence = HeroLocation.TOO_Close;
		if ( Vector3.Distance( Hero.transform.position, redPortal.gameObject.transform.position ) <=
			210.0f )
			heroPresence = HeroLocation.Close;
		else if ( Hero.transform.position.z > UpperSplit.transform.position.z )
			heroPresence = HeroLocation.TOP_Lane;
		else if ( Hero.transform.position.z < LowerSplit.transform.position.z )
			heroPresence = HeroLocation.BOT_Lane;
		else
			heroPresence = HeroLocation.MID_Lane;
		if ( HeroLocation.TOO_Close == heroPresence )
			redPortal.DmgDamp = 16.555f * towersRemaining;
		else
			redPortal.DmgDamp = 10.0f * towersRemaining;
	}
	private void FixedUpdate()
	{
#if DEBUG
		second -= Time.fixedDeltaTime;
		temp += CalcSelfRecovery * Time.fixedDeltaTime;
		if ( second <= 0.0f )
		{
			temp2 = temp;
			second = 1.0f;
			temp = 0.0f;
		}
#endif
		if ( !lazer )
			return;
		if ( 0 == targets.Count || !targets[ 0 ] || !targets[ 0 ].gameObject.GetComponent<Info>().
			Alive )
		{
			Nil();
			if ( targets.Count >= 1 && !targets[ 0 ].gameObject.GetComponent<Info>().Alive )
				AttackRange_triggerExit( targets[ 0 ].gameObject );
		}
		if ( ( HeroLocation.TOO_Close == heroPresence || huntHero && HeroLocation.Close ==
			heroPresence ) && Hero.GetComponent<Info>().Alive )
		{
			portalLazer.transform.LookAt( Hero.transform );
			lazer.autofire = true;
		}
		else if ( targets.Count > 0 )
		{
			portalLazer.transform.LookAt( targets[ 0 ] );
			lazer.autofire = true;
		}
		else if ( HeroLocation.Close == heroPresence && Hero.GetComponent<Info>().Alive )
		{
			portalLazer.transform.LookAt( Hero.transform );
			lazer.autofire = true;
		}
		else
			lazer.autofire = false;
	}
	private void AttackRange_triggerExit( GameObject obj )
	{
		targets.Remove( obj.transform );
	}
	private void AttackRange_triggerEnter( GameObject obj )
	{
		if ( redPortal.Alive )
			if ( obj && obj.CompareTag( "Minion" ) )
				if ( Team.BLUE_TEAM == obj.GetComponent<Info>().team )
					targets.Add( obj.transform );
	}
	private void SetupMinion( Object _minion, Path lane, Team team )
	{
		min_go = _minion as GameObject;
		min_go.GetComponent<MinionMovement>().ChangeLane( lane );
		nma = min_go.GetComponent<NavMeshAgent>();
		nma.enabled = true;
		nma.destination = bluePortal.gameObject.transform.position;
	}
	private DangerLevel GetTriggered( float _num )
	{
		if ( dangerTreshold >= 90.0f )
			return DangerLevel.EXTREME;
		else if ( dangerTreshold >= 75.0f )
			return DangerLevel.CRITICAL;
		else if ( _num >= 60.0f )
			return DangerLevel.HIGH;
		else if ( _num >= 45.0f )
			return DangerLevel.MODERATE;
		else if ( _num >= 30.0f )
			return DangerLevel.MEDIUM;
		else if ( _num >= 12.0f )
			return DangerLevel.LOW;
		else
			return DangerLevel.MINIMAL;
	}
	private void Nil()
	{
		for ( int i = 0; i < targets.Count; ++i )
			if ( !( targets[ i ] && targets[ i ].gameObject.activeInHierarchy ) )
				targets.RemoveAt( i-- );
	}
}