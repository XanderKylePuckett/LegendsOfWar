﻿using UnityEngine;
public abstract class AbilityQBase : AbilityBase
{
    protected override void Update()
    {
        base.Update();
        if ((Input.GetKeyDown(KeyCode.Q) && !HeroCamScript.IsOnHero) || Input.GetKeyDown(KeyCode
            .Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            TryCast();
        ToggleCursor((Input.GetKey(KeyCode.Q) && !HeroCamScript.IsOnHero) || Input.GetKey(
            KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1));
    }
    protected override void AbilityActivate()
    {
        heroInfo.TheHeroAudio.PlayClip("HeroCastAbilityQ");
        base.AbilityActivate();
    }
}