using UnityEngine;
using System.Collections;


/// <summary>
/// this demo is identical to the Tween Chain demo except that it uses a TweenFlow to show how you can overlap
/// tweens with a TweenFlow
/// </summary>
public class TweenFlowGUI : BaseDemoGUI
{
	// we have 4 cubes setup
	public Transform[] cubes;
	
	
	void Start()
	{
		// create a TweenConfig that we will use on all 4 cubes
		var config = new GoTweenConfig()
			.setEaseType( GoEaseType.QuadIn ) // set the ease type for the tweens
			.materialColor( Color.magenta ) // tween the material color to magenta
			.eulerAngles( new Vector3( 0, 360, 0 ) ) // do a 360 rotation
			.position( new Vector3( 2, 8, 0 ), true ) // relative position tween so it will be starting from the current location
			.setIterations( 2, GoLoopType.PingPong ); // 2 iterations with a PingPong loop so we go out and back
		
		// create the flow and set it to have 2 iterations
		var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 2 ) );
		
		// add a completion handler for the chain
		flow.setOnCompleteHandler( c => Debug.Log( "flow complete" ) );
		
		// create a Tween for each cube and add it to the flow
		var startTime = 0f;
		foreach( var cube in cubes )
		{
			var tween = new GoTween( cube, 0.5f, config );
			flow.insert( startTime, tween );
			
			// increment our startTime so that the next tween starts when this one is halfway done
			startTime += 1.0f;


			var tweenTwo = new GoTween( cube, 0.5f, config );
			flow.insert( startTime, tweenTwo );

			startTime += 0.25f;
		}
		
		_tween = flow;
	}

}
