using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BcWeapon : MonoBehaviour {
	[Header("Weapon Options")]
	public bool		autofire=true;
	[Tooltip("Add here any axis from the Input Settings to fire when using that axis")]
	public string	triggerAxis="Fire1";
	public bool		isSemiAutomatic=true;
	public float	horizontalSpread=0;
	public float	verticalSpread=0;
	public float	rateOfFire=1;
	public int		clipSize=10;
	public float	reloadTime=1;
	
	[Header("Bullet Options")]
	public GameObject bulletPrefab;
	public int		bulletsPerShot=1;
	public float	bulletLife=5;
	public Vector3	bulletAcceleration;
	public Vector3	bulletGlobalAcceleration;
	public float	bulletSpeed=5;
	public float	bulletSpeedDelta=0;
	public float	accelerationScale;
	
	[Header("Gizmo Options")]
	public 	bool 	showDisplayHandles=false;
	public float 	displayScale=1;
	public Vector3	displayOffset=-Vector3.one*10+Vector3.right*20;
	public float	currentAmmo;

	[Header("Sound Options")]
	public AudioClip shootSound;
	public AudioClip reloadingSound;
	public AudioClip reloadedSound;
	public AudioClip emptyClickSound;
	
	float	bulletMass=1f;
	bool	isReloading;
	float	shootTimer;
	float	reloadTimer;
	float	lastTriggerValue;
	bool	triggerPushed;
	
	// <BUGFIX: Dev Team #23>
	//AudioSource audioSource;
	// </BUGFIX: Dev Team #23>
	
	void Start () {
		isReloading=false;
		currentAmmo=clipSize;
		shootTimer=0;
	}
	
	void Update () {
		triggerPushed=Input.GetAxis(triggerAxis)>lastTriggerValue;
		lastTriggerValue=Input.GetAxis(triggerAxis);

		if (autofire || Input.GetAxis(triggerAxis)>0) {
			if (!isSemiAutomatic) {
				if (triggerPushed) {
					Shoot();
				}
			} else {
				Shoot();
			}
		}
		if (!isReloading   && shootTimer>0)  {
			shootTimer-=Time.deltaTime;
		}
		if (currentAmmo<=0 && shootTimer<=0) {
			Reload();
		}

		// <BUGFIX: Dev Team #23>
		//foreach (AudioSource source in GetComponents<AudioSource>()) {
		//	if (!source.isPlaying) {
		//		Destroy(source);
		//	}
		//}
		// </BUGFIX: Dev Team #23>
	}
	
	public void Reload() {
		if (!isReloading) {
			// <BUGFIX: Dev Team #23>
			//audioSource =gameObject.AddComponent<AudioSource>();
			//audioSource.clip=reloadingSound;
			//audioSource.Play();
			AudioManager.PlaySoundEffect( reloadingSound, transform.position );
			// </BUGFIX: Dev Team #23>
			isReloading=true;
 		}
		shootTimer=0;
		reloadTimer+=Time.deltaTime;
		currentAmmo=-reloadTimer/reloadTime*clipSize;
		if (currentAmmo<=-clipSize) {
			currentAmmo=clipSize;
			isReloading=false;
			reloadTimer=0;
			shootTimer=0;
			// <BUGFIX: Dev Team #23>
			//audioSource =gameObject.AddComponent<AudioSource>();
			//audioSource.clip=reloadedSound;
			//audioSource.Play();
			AudioManager.PlaySoundEffect( reloadedSound, transform.position );
			// </BUGFIX: Dev Team #23>
		}
	}
	
	//Pull the trigger: shoots if the next bullet is ready
	public void Shoot() {
		if (!isReloading && currentAmmo>0 && shootTimer<=0) {
			currentAmmo--;
			shootTimer+=1/rateOfFire;
			SpawnBullet();
			// <BUGFIX: Dev Team #23>
			//audioSource =gameObject.AddComponent<AudioSource>();
			//audioSource.clip=shootSound;
			//audioSource.Play();
			AudioManager.PlaySoundEffect( shootSound, transform.position );
			// </BUGFIX: Dev Team #23>
		}
		if (isReloading && triggerPushed && !isSemiAutomatic) {
			// <BUGFIX: Dev Team #23>
			//audioSource =gameObject.AddComponent<AudioSource>();
			//audioSource.clip=emptyClickSound;
			//audioSource.Play();
			AudioManager.PlaySoundEffect( emptyClickSound, transform.position );
			// </BUGFIX: Dev Team #23>
		}
	}
	
	//Force shoot: even if the bullet is not ready yet
	public void SpawnBullet() {
		for (int i = 0; i < bulletsPerShot; i++) {
			GameObject newBullet=(GameObject)Instantiate(bulletPrefab, transform.position, transform.rotation);
			Rigidbody rigidBody=newBullet.AddComponent<Rigidbody>();
			rigidBody.mass=bulletMass;
			rigidBody.drag=0;
			rigidBody.useGravity=false;
			rigidBody.velocity=
				 transform.forward	*(bulletSpeed+bulletSpeedDelta	*(Random.value-0.5f)*2)
				+transform.up		*(verticalSpread				*(Random.value-0.5f)*2)
				+transform.right	*(horizontalSpread				*(Random.value-0.5f)*2);
			
			ConstantForce force=newBullet.AddComponent<ConstantForce>();
			force.force=(bulletAcceleration.x*transform.forward
						  +bulletAcceleration.y*transform.up
						  +bulletAcceleration.z*transform.right)*accelerationScale;
			force.force+=(bulletGlobalAcceleration)*accelerationScale;
			force.force*=50f;
			
			newBullet.AddComponent<CCDBullet>().life=bulletLife;
		}
	}
	
	public Vector3 DisplayPosition(bool swap=false) {
		Vector3 result = DisplayOffset(swap)+transform.position;
		return result;
	}
	
	public Vector3 DisplayOffset(bool swap=false) {
		Vector3 result = displayOffset.z * transform.forward * transform.lossyScale.z
       					+displayOffset.y * transform.up      * transform.lossyScale.y
       					+displayOffset.x * transform.right   * transform.lossyScale.x;
		if (!swap) {return result;} else {return new Vector3(result.z, result.y, result.x);}
	}
	
	void OnDrawGizmosSelected() {
		DrawGizmos();
	}
	
	void DrawGizmos() {
		if (this.enabled) {
			//Shoot cone display
			Mesh hull=new Mesh();
			hull.Clear();
			Vector3[] vertexMatrix=new Vector3[8];
			Vector3[] dots1, dots2;
			dots1=DrawShot(transform.forward*(bulletSpeed+bulletSpeedDelta), bulletLife, Color.gray);
			dots2=DrawShot(transform.forward*(bulletSpeed-bulletSpeedDelta), bulletLife, Color.gray);
			System.Array.Copy(dots1, 0, vertexMatrix, 0, dots1.Length);
			System.Array.Copy(dots2, 0, vertexMatrix, dots1.Length, dots2.Length);
			hull.vertices=vertexMatrix;
			Vector3[] except=null;
			DrawMesh(hull, except);
			Gizmos.DrawWireSphere(DrawTrajectory(transform.position, transform.forward*(bulletSpeed), bulletLife, Color.yellow),0.3f);
	
			//Scale values
			float width=1/rateOfFire*clipSize*displayScale;
			float height=displayScale;
			
			//Shooting time display
			for (float i = 0; i < clipSize/rateOfFire; i+=1) {
				float size=0.25f*height; 		Gizmos.color=Color.white;
				if (i%5==0)  {size=0.5f*height; Gizmos.color=Color.white;}
				if (i%10==0) {size=1.0f*height; Gizmos.color=Color.white;}
				Gizmos.DrawLine(DisplayPosition()-transform.up*(height*0+0   )+transform.forward*i*displayScale,
								DisplayPosition()-transform.up*(height*0-size)+transform.forward*i*displayScale);
			}
			Gizmos.color=Color.white;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2-height/2)+transform.forward*0    		,
							DisplayPosition()-transform.up*(-height/2-height/2)+transform.forward*width);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2+height/2)+transform.forward*0    		,
							DisplayPosition()-transform.up*(-height/2+height/2)+transform.forward*width);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2-height/2)+transform.forward*width	,
							DisplayPosition()-transform.up*(-height/2+height/2)+transform.forward*width);
			Gizmos.color=Color.green;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2)+transform.forward*0,
							DisplayPosition()-transform.up*(-height/2)+transform.forward*Mathf.Max(0,currentAmmo+shootTimer*rateOfFire)/clipSize*width);
			
			//Clip size display
			for (float i = 0; i < clipSize; i+=1) {
				float size=0.15f*height; 		Gizmos.color=Color.white;
				if (i%5==0)  {size=0.5f*height; Gizmos.color=Color.white;}
				if (i%10==0) {size=1.0f*height; Gizmos.color=Color.white;}
				Gizmos.DrawLine(DisplayPosition()-transform.up*(+height*1+0   )+transform.forward*i*width/clipSize, 
								DisplayPosition()-transform.up*(+height*1-size)+transform.forward*i*width/clipSize);
			}
			Gizmos.color=Color.white;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(+height/2-height/2)+transform.forward*0    		,
							DisplayPosition()-transform.up*(+height/2-height/2)+transform.forward*width);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(+height/2+height/2)+transform.forward*0    		,
							DisplayPosition()-transform.up*(+height/2+height/2)+transform.forward*width);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(+height/2-height/2)+transform.forward*width	,
							DisplayPosition()-transform.up*(+height/2+height/2)+transform.forward*width);
			Gizmos.color=Color.yellow;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(+height/2)+transform.forward*0,
				            DisplayPosition()-transform.up*(+height/2)+transform.forward*Mathf.Max(0,currentAmmo)/clipSize*width);
			
			//Reload time display
			for (float i = 0; i < reloadTime; i+=1) {
				float size=0.15f*height; 		Gizmos.color=Color.white;
				if (i%5==0)  {size=0.5f*height; Gizmos.color=Color.white;}
				if (i%10==0) {size=1.0f*height; Gizmos.color=Color.white;}
				Gizmos.DrawLine(DisplayPosition()-transform.up*(-height*0+0   )-transform.forward*i*displayScale, 
								DisplayPosition()-transform.up*(-height*0-size)-transform.forward*i*displayScale);
			}
			Gizmos.color=Color.white;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2-height/2)-transform.forward*0		                 , 
							DisplayPosition()-transform.up*(-height/2-height/2)-transform.forward*reloadTime*displayScale);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2+height/2)-transform.forward*0		                 , 
							DisplayPosition()-transform.up*(-height/2+height/2)-transform.forward*reloadTime*displayScale);
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2-height/2)-transform.forward*reloadTime*displayScale, 
							DisplayPosition()-transform.up*(-height/2+height/2)-transform.forward*reloadTime*displayScale);
			Gizmos.color=Color.red;
			Gizmos.DrawLine(DisplayPosition()-transform.up*(-height/2         )-transform.forward*0,
							DisplayPosition()-transform.up*(-height/2         )-transform.forward*reloadTimer*displayScale);
			
			//GUI context line
			Gizmos.color=Color.white;
			Gizmos.DrawLine(transform.position,
							transform.position+transform.up*(height*2)*IsOverCannon());
			Gizmos.DrawLine(transform.position+transform.up*(height*2)*IsOverCannon(),
							DisplayPosition() -transform.up*(height*2)*IsOverCannon());
			Gizmos.DrawLine(DisplayPosition() -transform.up*(height*2)*IsOverCannon(),
							DisplayPosition());
			
			//Cannon position & shoot direction display
			if (autofire) {Gizmos.color=new Color(0.75f ,0.5f, 0.5f, 1);} else {Gizmos.color=Color.white;}
			Gizmos.DrawLine(transform.position+displayScale*transform.up,			transform.position-displayScale*transform.up);
			Gizmos.DrawLine(transform.position+displayScale*transform.right,		transform.position-displayScale*transform.right);
			Gizmos.DrawLine(transform.position+displayScale*transform.forward*1.5f,	transform.position-displayScale*transform.forward);
			Gizmos.DrawLine(transform.position+displayScale*transform.forward*1.5f,	transform.position-transform.up*displayScale/2);
			Gizmos.DrawLine(transform.position+displayScale*transform.forward*1.5f,	transform.position+transform.up*displayScale/2);
			Gizmos.DrawLine(transform.position+displayScale*transform.forward*1.5f,	transform.position-transform.right*displayScale/2);
			Gizmos.DrawLine(transform.position+displayScale*transform.forward*1.5f,	transform.position+transform.right*displayScale/2);
			Gizmos.DrawWireSphere(transform.position,0.475f*displayScale);
			
			//Bullets per shot display
			Gizmos.color=Color.cyan;
			for (float i = 0; i < bulletsPerShot; i++) {
				float size=0.5f;
				if (i%5==0)  {size=1.0f;}
				if (i%10==0) {size=2.0f;}
				Gizmos.DrawLine(transform.position									  +transform.up*i/bulletsPerShot*displayScale*2,
								transform.position+transform.forward*size*displayScale+transform.up*i/bulletsPerShot*displayScale*2);
			}
				
			DestroyImmediate(hull);
		}
	}
	
	public int IsOverCannon() {
		if (displayOffset.y>0) {return +1;}
		if (displayOffset.y<0) {return -1;}
		return 0;
	}
	
	Vector3 DrawTrajectory(Vector3 pos, Vector3 vel, float time, Color color, bool draw=true) {
		Gizmos.color=color;
		Vector3 nextPos;
		for (float i = 0; i < time; i+=Time.fixedDeltaTime) {
			nextPos=pos+Time.fixedDeltaTime*vel;
			if (draw) {Gizmos.DrawLine(pos, nextPos);}
			pos=nextPos;
			Vector3 accel=(bulletAcceleration.x*transform.forward+bulletAcceleration.y*transform.up+bulletAcceleration.z*transform.right+bulletGlobalAcceleration);
			vel+=(accel)*accelerationScale;
		}
		return pos;
	}
	
	public Vector3 ShotEnd(Vector3 initialSpeed, float time, Color color) {
		return DrawTrajectory(transform.position, initialSpeed, time, color, false);
	}
	
	Vector3[] DrawShot(Vector3 initialSpeed, float time, Color color, bool draw=true) {
		Vector3[] res=new Vector3[4];
		res[0]=DrawTrajectory(transform.position, initialSpeed -transform.up*verticalSpread -transform.right*horizontalSpread, time, color, draw);
		res[1]=DrawTrajectory(transform.position, initialSpeed +transform.up*verticalSpread -transform.right*horizontalSpread, time, color, draw);
		res[2]=DrawTrajectory(transform.position, initialSpeed +transform.up*verticalSpread +transform.right*horizontalSpread, time, color, draw);
		res[3]=DrawTrajectory(transform.position, initialSpeed -transform.up*verticalSpread +transform.right*horizontalSpread, time, color, draw);
		Gizmos.color=Color.white;
		return res;
	}
	
	void ExceptionLine(Vector3 source, Vector3 destination) {
		bool skip = true;
		
		Vector3 s=transform.worldToLocalMatrix.MultiplyVector(source);
		Vector3 d=transform.worldToLocalMatrix.MultiplyVector(destination);
		
		if (RoughlyEqual(s.x,d.x) || RoughlyEqual(s.y,d.y) || RoughlyEqual(s.z,d.z)) {skip=false;}
		if (RoughlyEqual(s.x,d.x) ^  RoughlyEqual(s.y,d.y) ^  RoughlyEqual(s.z,d.z)) {skip=true;}
		
		if (!skip) {
			Gizmos.DrawLine(source, destination);
		}
	}

	void DrawMesh(Mesh mesh, Vector3[] exception) {
		foreach (Vector3 vert in mesh.vertices) {
			Gizmos.color=Color.white;
			foreach (Vector3 check in mesh.vertices) {
				ExceptionLine(vert, check);
			}
		}
	}
	
	static bool RoughlyEqual(float a, float b) {
	   float treshold = 0.01f;
	   return (Mathf.Abs(a-b)< treshold);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(BcWeapon))] 
public class BcWeaponEditor : Editor {
	public SerializedProperty test;
	public SerializedProperty bulletPrefab;
	public SerializedProperty bulletSpeed;
	public SerializedProperty bulletSpeedDelta;
	public SerializedProperty bulletLife;
	public SerializedProperty bulletAcceleration;
	public SerializedProperty Offset;
	public SerializedProperty displayScale;
	public SerializedProperty displayOffset;
	public SerializedProperty gizmosFlag;
	public SerializedProperty clipSize;
	public SerializedProperty horizontalSpread;
	public SerializedProperty verticalSpread;
	public SerializedProperty bulletsPerShot;
	public SerializedProperty showDisplayHandles;
	public SerializedProperty bulletGlobalAcceleration;
	public SerializedProperty rateOfFire;
	public SerializedProperty reloadTime;
	public SerializedProperty isSemiAutomatic;
	
	public SerializedProperty shootSound;
	public SerializedProperty reloadingSound;
	public SerializedProperty reloadedSound;
	public SerializedProperty emptyClickSound;
	
	public bool speedEditMode;
	public bool lifeEditMode;
	
	void SerializeProperties() {
		test=this.serializedObject.FindProperty("test");
		bulletPrefab=this.serializedObject.FindProperty("bulletPrefab");
		horizontalSpread=this.serializedObject.FindProperty("horizontalSpread");
		verticalSpread=this.serializedObject.FindProperty("verticalSpread");
		displayOffset=this.serializedObject.FindProperty("displayOffset");
		bulletAcceleration=this.serializedObject.FindProperty("bulletAcceleration");
		bulletSpeed=this.serializedObject.FindProperty("bulletSpeed");
		bulletSpeedDelta=this.serializedObject.FindProperty("bulletSpeedDelta");
		bulletLife=this.serializedObject.FindProperty("bulletLife");
		rateOfFire=this.serializedObject.FindProperty("rateOfFire");
		clipSize=this.serializedObject.FindProperty("clipSize");
		displayScale=this.serializedObject.FindProperty("displayScale");
		bulletsPerShot=this.serializedObject.FindProperty("bulletsPerShot");
		showDisplayHandles=this.serializedObject.FindProperty("showDisplayHandles");
		bulletGlobalAcceleration=this.serializedObject.FindProperty("bulletGlobalAcceleration");
		reloadTime=this.serializedObject.FindProperty("reloadTime");
		gizmosFlag=this.serializedObject.FindProperty("gizmosFlag");
		isSemiAutomatic=this.serializedObject.FindProperty("isSemiAutomatic");
		shootSound=this.serializedObject.FindProperty("shootSound");
		reloadingSound=this.serializedObject.FindProperty("reloadingSound");
		reloadedSound=this.serializedObject.FindProperty("reloadedSound");
		emptyClickSound=this.serializedObject.FindProperty("emptyClickSound");
	}
	
	void OnSceneGUI() {
		Vector3 offset, vec;
		BcWeapon tW = (BcWeapon) target;
		
		if (tW.enabled) {
			
			SerializeProperties();
			
			//Display position handle
			if (showDisplayHandles.boolValue) {
				offset=tW.transform.position;
				vec=-offset+Handles.PositionHandle(offset+tW.DisplayOffset(), tW.transform.rotation);
				displayOffset.vector3Value=tW.transform.worldToLocalMatrix.MultiplyVector(vec);
				
				displayScale.floatValue=Mathf.Max(0.1f, 
				                                  Handles.ScaleSlider(displayScale.floatValue, 
				                    offset+tW.DisplayOffset(),
				                    -tW.transform.up,
				                    tW.transform.rotation,
				                    0.75f*HandleUtility.GetHandleSize(offset+tW.DisplayOffset()), 0));
			}
			
			//Rate of fire handle (time scale)
			Handles.color=Color.white;
			offset=tW.DisplayPosition()+tW.transform.up*displayScale.floatValue/2+tW.transform.forward*0.5f*displayScale.floatValue;
			vec=-offset+Handles.Slider(offset+tW.transform.forward*1/rateOfFire.floatValue*tW.clipSize*displayScale.floatValue, tW.transform.forward, 0.75f*displayScale.floatValue, Handles.ConeCap, 0);
			rateOfFire.floatValue=tW.clipSize/(vec.magnitude/displayScale.floatValue);
			
			//Clip size handle
			Handles.color=Color.white;
			offset=tW.DisplayPosition()-tW.transform.up*displayScale.floatValue/2+tW.transform.forward*0.75f*displayScale.floatValue;
			vec=-offset+Handles.Slider(offset+tW.transform.forward*1/tW.rateOfFire*tW.clipSize*displayScale.floatValue, tW.transform.forward, 0.75f*displayScale.floatValue, Handles.SphereCap, 0);
			offset=tW.DisplayPosition()-tW.transform.up*displayScale.floatValue/2+tW.transform.forward*0.5f*displayScale.floatValue;
			vec=-offset+Handles.Slider(offset+tW.transform.forward*1/tW.rateOfFire*tW.clipSize*displayScale.floatValue, tW.transform.forward, 0.75f*displayScale.floatValue, Handles.CylinderCap, 0);
			clipSize.intValue=Mathf.RoundToInt(tW.rateOfFire*(vec.magnitude/displayScale.floatValue));
			
			//Reload time handle
			Handles.color=Color.gray;
			offset=tW.DisplayPosition()+tW.transform.up*displayScale.floatValue/2-tW.transform.forward*0.5f*displayScale.floatValue;
			vec=-offset+Handles.Slider(offset-tW.transform.forward*tW.reloadTime*displayScale.floatValue, -tW.transform.forward, 0.75f*displayScale.floatValue, Handles.ConeCap, 0);
			reloadTime.floatValue=vec.magnitude/displayScale.floatValue;
			
			//Update values
			this.serializedObject.ApplyModifiedProperties();
			
			//Lifetime display
			Handles.color=Color.white;
			Vector3 position=tW.ShotEnd(tW.bulletSpeed*tW.transform.forward, tW.bulletLife, Color.white);
			Handles.Label(position, Mathf.Round(100*tW.bulletLife)/100+" secs" + " | " +Mathf.Round(100*Vector3.Distance(tW.transform.position, position))/100+" m");
		}
	}
	
	Texture bulletcreatorLogo_;
	Texture bulletcreatorLogo {
		get {
			if (bulletcreatorLogo_==null) {
				string path=AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("BcWeapon")[0]).Replace("BcWeapon.cs","")+"Internal/Images/bulletcreator_logo.png";
				bulletcreatorLogo_ = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
			}
			return bulletcreatorLogo_;
		}
	}
	Texture pixelattoIcon_;
	Texture pixelattoIcon {
		get {
			if (pixelattoIcon_==null) {
				string path=AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("BcWeapon")[0]).Replace("BcWeapon.cs","")+"Internal/Images/pixelatto_icon.png";
				pixelattoIcon_ = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
			}
			return pixelattoIcon_;
		}
	}
	
	override public void OnInspectorGUI() {
		DrawTitle();
		DrawDefaultInspector();
	}
	
	void DrawTitle() {
		GUILayout.Space(8f);
		//if (bulletcreatorLogo!=null) {
			var headerRect = GUILayoutUtility.GetRect(Screen.width, 5.0f);
			headerRect.x=headerRect.x-16;
			headerRect.width = bulletcreatorLogo.width;
			headerRect.height = bulletcreatorLogo.height;
			GUILayout.Space( headerRect.height );
			GUI.DrawTexture( headerRect, bulletcreatorLogo);
			
			if (GUI.Button(new Rect(Screen.width-60, headerRect.y+4, headerRect.height*0.875f, headerRect.height*0.875f), new GUIContent(pixelattoIcon, "More Pixelatto Assets"))) {
				Application.OpenURL("http://www.pixelatto.com/");
			}
		//}
	}	
}
#endif