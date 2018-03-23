using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameters {

	//Game Physics Constants
	public static float BALL_SPEED = 12f;
	public static float FF_SPEED = 55f;
	public static float MIN_LAUNCH_Y = 0.15f;
	public static float GRAVITY_SCALE = 0.02f;
	public static float INTER_BALL_DELAY = 0.16f;
	public static float EPSILON = 0.02f;
	public static Vector2 MIN_LAUNCH_LEFT = new Vector2(-0.9f, MIN_LAUNCH_Y).normalized;
	public static Vector2 MIN_LAUNCH_RIGHT = new Vector2(0.9f, MIN_LAUNCH_Y).normalized;
	public static float TIME_FOR_MOVE = 0.5f;
	public static float[] startPositions = new float[] {11f, 10f, 9f, 8f, 7f, 6f, 5f, 4f, 3f};
	public static float LARGE_BLOCK_PROB = 0.6f;

	public static string APPLE_APP_ID = "1358118995";
	public static string APPLE_APP_URL = "itms-apps://itunes.apple.com/us/app/blonk/id1358118995";

}
