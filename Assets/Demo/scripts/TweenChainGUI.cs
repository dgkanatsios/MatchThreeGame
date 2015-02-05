using UnityEngine;
using System.Collections;


public class TweenChainGUI : BaseDemoGUI
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
			.position( new Vector3( 2, 8, 0 ), true ) // relative position tween so it will be start from the current location
			.setIterations( 2, GoLoopType.PingPong ); // 2 iterations with a PingPong loop so we go out and back
		
		// create the chain and set it to have 2 iterations
		var chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations( 2 ));
		
		// add a completion handler for the chain
		chain.setOnCompleteHandler( c => Debug.Log( "chain complete" ) );

		// create a Tween for each cube and it to the chain
		foreach( var cube in cubes )
		{
			var tween = new GoTween( cube, 0.5f, config );
			chain.append( tween );
		}
		
		_tween = chain;
	}

}
