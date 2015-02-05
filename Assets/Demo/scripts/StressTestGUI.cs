using UnityEngine;
using System.Collections;



public class StressTestGUI : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[1500];
	private Vector3[] origPos = new Vector3[1500];
	private GoTween[] tweens = new GoTween[1500];
	private PositionTweenProperty[] props = new PositionTweenProperty[1500];
	private Perlin noise = new Perlin();
	
	private float updateChange = 0.0f;
	private float timex;
	private float timey;
	private float timez;
	private Vector3 endPt;
	private int i;
	
	
	void Start()
	{
		Application.targetFrameRate = 60;
		
		Go.defaultEaseType = GoEaseType.Linear;
		Go.duplicatePropertyRule = GoDuplicatePropertyRuleType.None;
		Go.validateTargetObjectsEachTick = true;
		var sharedTweenConfig = new GoTweenConfig();
		
		for( i = 0; i < cubes.Length; i++ )
		{
			var cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
			Destroy( cube.GetComponent<BoxCollider>() );
			cube.transform.position = new Vector3( i * 0.1f - 10, cube.transform.position.y, i % 10 );
			cubes[i] = cube;
			origPos[i] = cube.transform.position;
			
			props[i] = new PositionTweenProperty( Vector3.zero );
			tweens[i] = new GoTween( cubes[i].transform, 1.0f, sharedTweenConfig );
			tweens[i].autoRemoveOnComplete = false;
			tweens[i].addTweenProperty( props[i] );
			tweens[i].pause();
			Go.addTween( tweens[i] );
		}
		
		StartCoroutine( runTest() );
	}
	
	
	private IEnumerator runTest()
	{
		var waiter = new WaitForSeconds( 1.0f );
		
		while( true )
		{
			for( var i = 0; i < cubes.Length; i++ )
			{
				timex = updateChange * 0.1365143f;
				timey = updateChange * 0.3365143f;
				timez = updateChange * 2.5564f;
				
				endPt.x = noise.Noise( timex ) * 100 + origPos[i].x;
				endPt.y = noise.Noise( timey ) * 100 + origPos[i].y;
				endPt.z = noise.Noise( timez ) * 100 + origPos[i].z;
				
				var tween = tweens[i];
				props[i].resetWithNewEndValue( endPt );
				tween.restart( true );
				
				updateChange += Time.deltaTime * 100;
			}
			
			yield return waiter;
		}
	}
	
	
	public class Perlin
	{
		// Original C code derived from
		// http://astronomy.swin.edu.au/~pbourke/texture/perlin/perlin.c
		// http://astronomy.swin.edu.au/~pbourke/texture/perlin/perlin.h
		const int B = 0x100;
		const int BM = 0xff;
		const int N = 0x1000;
	
		int[] p = new int[B + B + 2];
		float[,] g3 = new float [B + B + 2 , 3];
		float[,] g2 = new float[B + B + 2,2];
		float[] g1 = new float[B + B + 2];
	
		float s_curve(float t)
		{
			return t * t * (3.0F - 2.0F * t);
		}
		
		float lerp (float t, float a, float b)
		{
			return a + t * (b - a);
		}
	
		void setup (float value, out int b0, out int b1, out float r0, out float r1)
		{
	        float t = value + N;
	        b0 = ((int)t) & BM;
	        b1 = (b0+1) & BM;
	        r0 = t - (int)t;
	        r1 = r0 - 1.0F;
		}
		
		float at2(float rx, float ry, float x, float y) { return rx * x + ry * y; }
		float at3(float rx, float ry, float rz, float x, float y, float z) { return rx * x + ry * y + rz * z; }
	
		public float Noise(float arg)
		{
			int bx0, bx1;
			float rx0, rx1, sx, u, v;
			setup(arg, out bx0, out bx1, out rx0, out rx1);
			
			sx = s_curve(rx0);
			u = rx0 * g1[ p[ bx0 ] ];
			v = rx1 * g1[ p[ bx1 ] ];
			
			return(lerp(sx, u, v));
		}
	
		public float Noise(float x, float y)
		{
			int bx0, bx1, by0, by1, b00, b10, b01, b11;
			float rx0, rx1, ry0, ry1, sx, sy, a, b, u, v;
			int i, j;
			
			setup(x, out bx0, out bx1, out rx0, out rx1);
			setup(y, out by0, out by1, out ry0, out ry1);
			
			i = p[ bx0 ];
			j = p[ bx1 ];
			
			b00 = p[ i + by0 ];
			b10 = p[ j + by0 ];
			b01 = p[ i + by1 ];
			b11 = p[ j + by1 ];
			
			sx = s_curve(rx0);
			sy = s_curve(ry0);
			
			u = at2(rx0,ry0, g2[ b00, 0 ], g2[ b00, 1 ]);
			v = at2(rx1,ry0, g2[ b10, 0 ], g2[ b10, 1 ]);
			a = lerp(sx, u, v);
			
			u = at2(rx0,ry1, g2[ b01, 0 ], g2[ b01, 1 ]);
			v = at2(rx1,ry1, g2[ b11, 0 ], g2[ b11, 1 ]);
			b = lerp(sx, u, v);
			
			return lerp(sy, a, b);
		}
		
		public float Noise(float x, float y, float z)
		{
			int bx0, bx1, by0, by1, bz0, bz1, b00, b10, b01, b11;
			float rx0, rx1, ry0, ry1, rz0, rz1, sy, sz, a, b, c, d, t, u, v;
			int i, j;
			
			setup(x, out bx0, out bx1, out rx0, out rx1);
			setup(y, out by0, out by1, out ry0, out ry1);
			setup(z, out bz0, out bz1, out rz0, out rz1);
			
			i = p[ bx0 ];
			j = p[ bx1 ];
			
			b00 = p[ i + by0 ];
			b10 = p[ j + by0 ];
			b01 = p[ i + by1 ];
			b11 = p[ j + by1 ];
			
			t  = s_curve(rx0);
			sy = s_curve(ry0);
			sz = s_curve(rz0);
			
			u = at3(rx0,ry0,rz0, g3[ b00 + bz0, 0 ], g3[ b00 + bz0, 1 ], g3[ b00 + bz0, 2 ]);
			v = at3(rx1,ry0,rz0, g3[ b10 + bz0, 0 ], g3[ b10 + bz0, 1 ], g3[ b10 + bz0, 2 ]);
			a = lerp(t, u, v);
			
			u = at3(rx0,ry1,rz0, g3[ b01 + bz0, 0 ], g3[ b01 + bz0, 1 ], g3[ b01 + bz0, 2 ]);
			v = at3(rx1,ry1,rz0, g3[ b11 + bz0, 0 ], g3[ b11 + bz0, 1 ], g3[ b11 + bz0, 2 ]);
			b = lerp(t, u, v);
			
			c = lerp(sy, a, b);
			
			u = at3(rx0,ry0,rz1, g3[ b00 + bz1, 0 ], g3[ b00 + bz1, 2 ], g3[ b00 + bz1, 2 ]);
			v = at3(rx1,ry0,rz1, g3[ b10 + bz1, 0 ], g3[ b10 + bz1, 1 ], g3[ b10 + bz1, 2 ]);
			a = lerp(t, u, v);
			
			u = at3(rx0,ry1,rz1, g3[ b01 + bz1, 0 ], g3[ b01 + bz1, 1 ], g3[ b01 + bz1, 2 ]);
			v = at3(rx1,ry1,rz1,g3[ b11 + bz1, 0 ], g3[ b11 + bz1, 1 ], g3[ b11 + bz1, 2 ]);
			b = lerp(t, u, v);
			
			d = lerp(sy, a, b);
			
			return lerp(sz, c, d);
		}
		
		void normalize2(ref float x, ref float y)
		{
		   float s;
		
			s = (float)Mathf.Sqrt(x * x + y * y);
			x = y / s;
			y = y / s;
		}
		
		void normalize3(ref float x, ref float y, ref float z)
		{
			float s;
			s = (float)Mathf.Sqrt(x * x + y * y + z * z);
			x = y / s;
			y = y / s;
			z = z / s;
		}
		
		public Perlin()
		{
			int i, j, k;
			System.Random rnd = new System.Random();
		
		   for (i = 0 ; i < B ; i++) {
			  p[i] = i;
			  g1[i] = (float)(rnd.Next(B + B) - B) / B;
		
			  for (j = 0 ; j < 2 ; j++)
				 g2[i,j] = (float)(rnd.Next(B + B) - B) / B;
			  normalize2(ref g2[i, 0], ref g2[i, 1]);
		
			  for (j = 0 ; j < 3 ; j++)
				 g3[i,j] = (float)(rnd.Next(B + B) - B) / B;
				
		
			  normalize3(ref g3[i, 0], ref g3[i, 1], ref g3[i, 2]);
		   }
		
		   while (--i != 0) {
			  k = p[i];
			  p[i] = p[j = rnd.Next(B)];
			  p[j] = k;
		   }
		
		   for (i = 0 ; i < B + 2 ; i++) {
			  p[B + i] = p[i];
			  g1[B + i] = g1[i];
			  for (j = 0 ; j < 2 ; j++)
				 g2[B + i,j] = g2[i,j];
			  for (j = 0 ; j < 3 ; j++)
				 g3[B + i,j] = g3[i,j];
		   }
		}
	}

}
