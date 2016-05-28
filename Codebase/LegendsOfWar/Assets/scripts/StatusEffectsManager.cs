﻿using UnityEngine;
using System.Collections.Generic;
public class StatusEffectsManager : MonoBehaviour
{
	private static StatusEffectsManager inst = null;

	private SortedList<string, Effect> stats = new SortedList<string, Effect>();
	private SortedList<string, SortedList<string, Effect>> objects = new SortedList<string,
		SortedList<string, Effect>>();
	public static StatusEffectsManager Instance
	{
		get
		{
			if ( !inst )
			{
				inst = FindObjectOfType<StatusEffectsManager>();
				if ( !inst )
					inst = new GameObject( "StatusManager" ).AddComponent<StatusEffectsManager>();
			}
			return inst;
		}
	}
	private void Awake()
	{
		if ( inst )
			Destroy( this );
		else
			inst = this;
	}
	public void AddStatus( string _nameKey, Effect _effect )
	{
		if ( !objects.ContainsKey( _nameKey ) )
		{
			stats.Clear();
			stats = new SortedList<string, Effect>();
			objects.Add( _nameKey, stats );
		}
		if ( objects[ _nameKey ].ContainsKey( _effect.m_name ) )
		{
			if ( _effect.m_stackable )
				objects[ _nameKey ][ _effect.m_name ].m_stacks++;
			else
				objects[ _nameKey ][ _effect.m_name ].Refresh();
		}
		else
			objects[ _nameKey ].Add( _effect.m_name, _effect );
	}
	public IList<Effect> GetMyStatus( string _nameKey )
	{
		if ( objects.ContainsKey( _nameKey ) )
			return objects[ _nameKey ].Values;
		else
			return null;
	}
	public bool Expired( string _nameKey, Effect _effect )
	{
		return objects[ _nameKey ].Remove( _effect.m_name );
	}
	public bool CheckSkill( string _nameKey, string _skillName )
	{
		if ( objects.ContainsKey( _nameKey ) )
			return objects[ _nameKey ].ContainsKey( _skillName );
		else
			return objects.ContainsKey( _nameKey );
	}
	public int GetStacks( string _nameKey, string _skillName )
	{
		if ( CheckSkill( _nameKey, _skillName ) )
			return objects[ _nameKey ][ _skillName ].m_stacks;
		else
			return 0;
	}
}