﻿public class SiegeMinionInfo : MinionInfo
{
	public int clipSize = 10;
	public float reloadTime = 1.0f;
	public int bulletsPerShot = 1;
}
#region OLD_CODE
#if false
using UnityEngine;
using System.Collections;

public class SiegeMinionInfo : MinionInfo {
    [SerializeField]
    int clipSize = 10;

    public int ClipSize
    {
        get { return clipSize; }
        set { clipSize = value; }
    }
    [SerializeField]
    float reloadTime = 1;

    public float ReloadTime
    {
        get { return reloadTime; }
        set { reloadTime = value; }
    }

    [SerializeField]
    int bulletsPerShot = 1;

    public int BulletsPerShot
    {
        get { return bulletsPerShot; }
        set { bulletsPerShot = value; }
    }
}

#endif
#endregion //OLD_CODE