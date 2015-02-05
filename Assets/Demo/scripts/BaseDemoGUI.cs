using UnityEngine;
using System;
using System.Collections;



public class BaseDemoGUI : MonoBehaviour
{
	protected AbstractGoTween _tween;
	protected float _tweenTimeScale = 1;

	
	protected virtual void OnGUI()
	{
		if( _tween == null )
			return;
		
		GUILayout.Label( "elapsed: " + string.Format( "{0:0.##}", _tween.totalElapsedTime ) );
		
		
		if( GUILayout.Button( "play" ) )
			_tween.play();
		
		if( GUILayout.Button( "pause" ) )
			_tween.pause();
		
		if( GUILayout.Button( "reverse" ) )
			_tween.reverse();
		
		if( GUILayout.Button( "restart" ) )
			_tween.restart();
		
		if( GUILayout.Button( "play backwards" ) )
			_tween.playBackwards();
		
		if( GUILayout.Button( "play forward" ) )
			_tween.playForward();
		
		if( GUILayout.Button( "complete" ) )
			_tween.complete();
		
		GUILayout.Label( "Time Scale: " + string.Format( "{0:0.##}", _tween.timeScale ) );
		var newTweenTimeScale = GUILayout.HorizontalSlider( _tweenTimeScale, 0, 3 );
		if( newTweenTimeScale != _tweenTimeScale )
		{
			_tweenTimeScale = newTweenTimeScale;
			_tween.timeScale = _tweenTimeScale;
		}
		
		easeTypesGUI();
	}
	
	
	protected void easeTypesGUI()
	{
		// ease section. only available for Tweens
		if( _tween is GoTween )
		{
			GUILayout.BeginArea( new Rect( Screen.width - 200, 0, 100, Screen.height ) );
			
			GUILayout.Label( "Ease Types" );
			
			var allEaseTypes = Enum.GetValues( typeof( GoEaseType ) );
			var midway = Mathf.Round( allEaseTypes.Length / 2 );
			
			for( var i = 0; i < allEaseTypes.Length; i++ )
			{
				var ease = allEaseTypes.GetValue( i );
				
				
				if( i == midway )
				{
					GUILayout.EndArea();
					GUILayout.BeginArea( new Rect( Screen.width - 100, 0, 100, Screen.height ) );
				}
				
				if( GUILayout.Button( ease.ToString() ) )
					((GoTween)_tween).easeType = (GoEaseType)ease;
			}
			
			GUILayout.EndArea();
		}
	}

}
