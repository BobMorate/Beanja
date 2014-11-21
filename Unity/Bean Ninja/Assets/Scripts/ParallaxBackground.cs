using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour {
	public Vector2 parallaxSpeed;
	private Material mat;
	private float xMultiply;
	private float yMultiply;

	// Use this for initialization
	void Start () {
		mat = renderer.material;
		xMultiply = mat.mainTextureScale.x/transform.localScale.x;
		yMultiply = mat.mainTextureScale.y/transform.localScale.y;
	}
	
	// Update is called once per frame
	void Update () {
		mat.mainTextureOffset = new Vector2(transform.position.x * xMultiply * parallaxSpeed.x, transform.position.y * yMultiply * parallaxSpeed.y);
	}
}
