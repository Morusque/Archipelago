using UnityEngine;
using System.Collections;

public class gradient : MonoBehaviour {

	public Camera cam;

	public float minAlpha = 0.0f;
	public float maxAlpha = 1.0f;

	void Start () {
		updateColors ();
	}

	public void updateColors() {
		Color startColor = new Color(cam.backgroundColor.r,cam.backgroundColor.g,cam.backgroundColor.b,maxAlpha);
		Color endColor = new Color(cam.backgroundColor.r,cam.backgroundColor.g,cam.backgroundColor.b,minAlpha);
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		var colors = new Color[mesh.vertices.Length];
		colors[0] = endColor;
		colors[1] = startColor;
		colors[2] = endColor;
		colors[3] = startColor;
		mesh.colors = colors;
	}
	
	void Update () {
	}
}
