namespace SomeRandom
{
	public class UnityRandom : IRandom
	{
		public int Range(int min, int max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public float Range(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}
	}
}
