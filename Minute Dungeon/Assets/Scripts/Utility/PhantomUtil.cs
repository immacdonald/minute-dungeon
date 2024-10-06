using UnityEngine;

public static class PhantomUtil {

	public const float marginOfError = 0.000001f;

	public static void VisualizeLocation(float x, float y, Color color) {
		VisualizeLocation(new Vector2(x,y), color);
	}

	public static void VisualizeLocation(Vector2 l, Color color) {
		Debug.DrawLine(new Vector3(l.x - 0.5f, l.y + 0.5f), new Vector3(l.x + 0.5f, l.y + 0.5f), color);
		Debug.DrawLine(new Vector3(l.x - 0.5f, l.y + 0.5f), new Vector3(l.x - 0.5f, l.y - 0.5f), color);
		Debug.DrawLine(new Vector3(l.x + 0.5f, l.y - 0.5f), new Vector3(l.x + 0.5f, l.y + 0.5f), color);
		Debug.DrawLine(new Vector3(l.x + 0.5f, l.y - 0.5f), new Vector3(l.x - 0.5f, l.y - 0.5f), color);
	}
		
	public static void VisualizeBetweenLocations(Vector2 startPoint, Vector2 endPoint, int steps, int divergence) {
		Debug.DrawLine (startPoint, endPoint, Color.red, Mathf.Infinity);
		float distance = 0; // From 0.0 to 1.0.
		if (steps <= 0) {
			return;		
		}
		float distancePerStep = 1f / steps; 
		Vector2 v = Vector2.zero;
		Vector2 lastV = Vector2.zero;

		while (distance <= 1 + marginOfError) {
			if (distance <= 0.8f + marginOfError && distance != 0) {
				v = RoundVector (Vector2.Lerp (startPoint, endPoint, distance)) + new Vector2(0, SeedManager.Instance.dungeonSeed.random.Next(-divergence, divergence));
			} else {
				v = FloorVector (Vector2.Lerp (startPoint, endPoint, distance));
			}
			VisualizeLocation (v, Color.blue);

			if (distance != 0) {
				Debug.DrawLine (lastV, new Vector2 (v.x, lastV.y), Color.cyan, Mathf.Infinity);
				Debug.DrawLine (new Vector2(v.x, lastV.y), v, Color.blue, Mathf.Infinity);
			}
			distance += (distance > 1 + marginOfError) ? 0 : distancePerStep;
				
			lastV = v;
		}
	}


	/// <summary>
	/// Gets the points between two locations. Used locally.
	/// </summary>
	public static Vector2[] GetPointsBetweenLocations(Vector2 startPoint, Vector2 endPoint, int steps, int divergence, bool visualize)
	{
		// Ensure we have at least two points (start and end)
		if (steps < 2)
		{
			Debug.LogError("Steps must be at least 2 to get points between locations");
			return null;
		}

		// Optional visualization step
		if (visualize)
		{
			VisualizeBetweenLocations(startPoint, endPoint, steps, divergence);
		}

		// Create array to hold all points, including start and end
		Vector2[] points = new Vector2[steps];

		// Calculate distance per step as a fraction of the total line length
		float distancePerStep = 1f / (steps - 1);

		// Iterate and generate points between start and end
		for (int i = 0; i < steps; i++)
		{
			// Linearly interpolate between start and end points
			Vector2 point = Vector2.Lerp(startPoint, endPoint, distancePerStep * i);

			// Add some random divergence if requested
			if (divergence != 0)
			{
				point += new Vector2(0, SeedManager.Instance.dungeonSeed.Next(-divergence, divergence));
			}

			// Store point in the array
			points[i] = RoundVector(point);
		}

		return points;
	}


	public static Vector2 FloorVector(Vector2 v) {			
		v.x = Mathf.Floor (v.x);
		v.y = Mathf.Floor (v.y);
		return v;
	}
	public static Vector2 CeilVector(Vector2 v) {			
		v.x = Mathf.Ceil (v.x);
		v.y = Mathf.Ceil (v.y);
		return v;
	}
	public static Vector2 RoundVector(Vector2 v) {			
		v.x = Mathf.Round (v.x);
		v.y = Mathf.Round (v.y);
		return v;
	}

	public static Vector2 Vector2Range(Vector2 min, Vector2 max) {
		return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	}
}
