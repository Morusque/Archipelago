using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

	// stored infos
	public int type = 0;
	public int shape = 0;
	public int rotation = 0;

	// general info
	public string name = "";

	Vector2[] contour;

	void Start () {
		updateShape();
	}

	void updateShape() {
		// full tile
		if (shape==0) {
			contour = new Vector2[4];
			contour[0] = new Vector2(-0.5f,-0.5f);
			contour[1] = new Vector2(-0.5f,+0.5f);
			contour[2] = new Vector2(+0.5f,+0.5f);
			contour[3] = new Vector2(+0.5f,-0.5f);
		}
		// 45° tiles
		if (shape==1) {
			contour = new Vector2[3];
			contour[0] = new Vector2(-0.5f,+0.5f);
			contour[1] = new Vector2(+0.5f,+0.5f);
			contour[2] = new Vector2(+0.5f,-0.5f);
		}
		// 60° tiles
		if (shape==2) {
			contour = new Vector2[3];
			contour[0] = new Vector2(-0.5f,-0.5f);
			contour[1] = new Vector2(-0.5f,+0.5f);
			contour[2] = new Vector2(+0.0f,+0.5f);
		}
		if (shape==3) {
			contour = new Vector2[4];
			contour[0] = new Vector2(-0.5f,-0.5f);
			contour[1] = new Vector2(-0.5f,+0.5f);
			contour[2] = new Vector2(+0.5f,+0.5f);
			contour[3] = new Vector2(+0.0f,-0.5f);
		}
		if (shape==4) {
			contour = new Vector2[3];
			contour[0] = new Vector2(+0.5f,-0.5f);
			contour[1] = new Vector2(+0.5f,+0.5f);
			contour[2] = new Vector2(-0.0f,+0.5f);
		}
		if (shape==5) {
			contour = new Vector2[4];
			contour[0] = new Vector2(+0.5f,-0.5f);
			contour[1] = new Vector2(+0.5f,+0.5f);
			contour[2] = new Vector2(-0.5f,+0.5f);
			contour[3] = new Vector2(-0.0f,-0.5f);
		}
		setRageSpline(contour);
	}

	public void setRotation(int r) {
		this.rotation = r;
		this.transform.eulerAngles = new Vector3(0,0,rotation*360.0f/4.0f);
	}

	public void setShape(int s) {
		this.shape = s;
		updateShape();
	}

	void Update () {
	
	}

	public TileInfos getTileInfos() {
		return new TileInfos(type,shape,rotation);
	}

	void setRageSpline(Vector2[] splinePoints) {
		IRageSpline rageSpline = GetComponent<RageSpline>();
		rageSpline.ClearPoints();
		for (int i=0;i<splinePoints.Length;i++) {
			rageSpline.AddPoint(0, new Vector3(splinePoints[i].x,splinePoints[i].y,0),new Vector3(0,0,0));
		}
		rageSpline.RefreshMesh();
	}

}
