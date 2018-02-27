﻿using UnityEngine;
using System.Collections.Generic;
public class SiegeMinionAttack : AttackScript
{
    [SerializeField]
    private List<Transform> targets;
    private ProximityCompare poo = new ProximityCompare();
    private BcWeapon weaponDetails;
    private Info targ;
    private SiegeMinionInfo sMinioninfo;
    private MinionMovement movement;
    private void Start()
    {
        sMinioninfo = GetComponent<SiegeMinionInfo>();
        attackTrigger.CreateTrigger(sMinioninfo.AgroRange);
        attackTrigger.TriggerEnter += AttackTriggerEnter;
        attackTrigger.TriggerExit += AttackTriggerExit;
        targets = new List<Transform>();
        movement = GetComponent<MinionMovement>();
        weaponDetails = weapon.GetComponentInChildren<BcWeapon>();
        if (EnemyAIManager.upgradeSiege)
        {
            weaponDetails.reloadTime = sMinioninfo.reloadTime * 0.85f;
            weaponDetails.clipSize = sMinioninfo.clipSize * 2;
            weaponDetails.rateOfFire = sMinioninfo.AttackSpeed * 1.2f;
            weaponDetails.bulletsPerShot = sMinioninfo.bulletsPerShot + 2;
            weaponDetails.SiegeProjectileDamage = sMinioninfo.Damage * 1.5f;
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed = sMinioninfo.MovementSpeed * 1.8f;
        }
        else
        {
            weaponDetails.reloadTime = sMinioninfo.reloadTime;
            weaponDetails.clipSize = sMinioninfo.clipSize;
            weaponDetails.rateOfFire = sMinioninfo.AttackSpeed;
            weaponDetails.bulletsPerShot = sMinioninfo.bulletsPerShot;
            weaponDetails.SiegeProjectileDamage = sMinioninfo.Damage;
        }
        weaponDetails.currentAmmo = 0.0f;
    }
    private void Update()
    {
        if (EnemyAIManager.huntHero)
        {
            movement.Disengage();
            if (Vector3.Distance(gameObject.transform.position, GameManager.Instance.Player.
                transform.position) <= sMinioninfo.AgroRange + 30.0f)
                movement.SetTarget(GameManager.Instance.Player.transform, sMinioninfo.AgroRange);
        }
        if (0 == targets.Count || !targets[0] || !targets[0].gameObject.GetComponent<Info>().
            Alive)
        {
            movement.Disengage();
            targets.RemoveAll(item => (!item || !item.gameObject.activeInHierarchy));
            if (1 <= targets.Count && !targets[0].gameObject.GetComponent<Info>().Alive)
                AttackTriggerExit(targets[0].gameObject);
        }
        else if (movement.InCombat && movement.WithinRange)
        {
            weapon.transform.LookAt(targets[0].position);
            weaponDetails.Shoot();
        }
        else if (!movement.InCombat)
            movement.SetTarget(targets[0], sMinioninfo.Range);
    }
    private void AttackTriggerEnter(GameObject obj)
    {
        if (isActiveAndEnabled)
            if (obj && obj.activeInHierarchy)
            {
                targ = obj.GetComponent<Info>();
                if (targ)
                    if (targ.team != sMinioninfo.team)
                        targets.Add(obj.transform);
            }
    }
    private void AttackTriggerExit(GameObject obj)
    {
        targets.Remove(obj.transform);
        if (2 < targets.Count)
        {
            targets.Sort(1, targets.Count - 1, poo);
            targets.Reverse(1, targets.Count - 1);
        }
    }
}