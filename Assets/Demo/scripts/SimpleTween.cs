using UnityEngine;
using System.Collections;


public class SimpleTween : BaseDemoGUI
{
	public Transform cube;
	
	
	void Start()
	{
		// setup a position and color tween that loops indefinitely. this will let us play with the
		// different ease types
		_tween = Go.to( cube, 4, new GoTweenConfig()
			.position( new Vector3( 9, 4, 0 ) )
			.materialColor( Color.green )
			.setIterations( -1, GoLoopType.PingPong ) );
	}

}
