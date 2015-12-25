using UnityEngine;
using System.Collections;

public class FireLight_Tst : MonoBehaviour {
	
	public float minIntensity;
	public float maxIntensity;
	Light LightRange;
	float random;


	// Use this for initialization
	void Start () {
		LightRange = GetComponent<Light> ();
		random = Random.Range (0.0f, 3.0f);
	}
	
	// Update is called once per frame
	void Update () {
		float noise = Mathf.PerlinNoise(random, Time.time);
		LightRange.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);


	}
}
