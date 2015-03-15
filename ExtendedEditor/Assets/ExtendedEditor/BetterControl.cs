﻿#if UNITY_EDITOR
using UnityEngine;
using Newtonsoft.Json;

public class BetterControl {

	public BetterWindow Window;

	public bool IsInitialized;

	public Vector2 Position;
	public Vector2 Size;
	[JsonIgnore]
	public Rect Rectangle {
		get {
			return new Rect( Position.x, Position.y, Size.x, Size.y );
		}
	}

	public BetterControl() { }

	public virtual void OnInitialize() {
		IsInitialized = true;
	}

	public virtual void OnDeserialized() { }

	public virtual void OnDestroy() {
		IsInitialized = false;
	}

	public virtual void Update() { }

	public virtual void OnGUI() { }

	#region Events
	public virtual void OnContextClick( Vector2 position ) { }
	public virtual void OnDoubleClick( EMouseButton button, Vector2 position ) { }
	public virtual void OnDragExited() { }
	public virtual void OnDragPerform( string[] paths, Vector2 position ) { }
	public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
	public virtual void OnScrollWheel( Vector2 delta ) { }
	#endregion
}
#endif