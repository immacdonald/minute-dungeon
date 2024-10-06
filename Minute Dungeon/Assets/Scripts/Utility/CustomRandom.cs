/// <summary>
/// A custom random class that has more features
/// </summary>
public class CustomRandom {
	public System.Random random;

	public CustomRandom(int seed) {
		this.random = new System.Random(seed);
	}

	public int Next(int a, int b) {
		return random.Next (a, b + 1);
	}
	public int Next(float a, float b) {
		return random.Next ((int)a, (int)b + 1);
	}
	public float NextFloat(int a, int b) {
		return random.Next (a, b) + (Next(0,10000) / 10000f);
	}
	public float NextFloat(float a, float b) {
		return random.Next ((int)a, (int)b + (Next(0,10000) / 10000));
	}

}