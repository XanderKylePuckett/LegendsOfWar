﻿public class TowerInfo : Info
{
    protected override void Start()
    {
        base.Start();
        Attacked += TowerAttacked;
        Destroyed += TowerDestroyed;
    }
    private void TowerAttacked()
    {
        AudioManager.PlaySoundEffect(AudioManager.sfxTowerAttacked, transform.position);
    }
    private void TowerDestroyed()
    {
        AudioManager.PlaySoundEffect(AudioManager.sfxTowerDestroyed, transform.position);
    }
}