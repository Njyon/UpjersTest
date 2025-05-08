using UnityEngine;

public static class Vector2Extension
{
	public static Vector3 ForceZAxes(this Vector2 vector, float LengthOnZ)
	{
		Vector3 vec = new Vector3(vector.x, vector.y, LengthOnZ);
		return vec;
	}
}
