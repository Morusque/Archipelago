using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uiUpdater : MonoBehaviour {

	public GameObject blockTypeInfo;
	public GameObject blockShapeInfo;
	public GameObject blockRotationInfo;

	GameObject[] exempleTiles;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateUi(editorManager manager) {
		if (manager.currentType==0) blockTypeInfo.GetComponent<Text>().text = "filled";
		if (manager.currentType==1) blockTypeInfo.GetComponent<Text>().text = "empty";
		if (exempleTiles!=null) for (int i=0;i<exempleTiles.Length;i++) Destroy(exempleTiles[i]);
		exempleTiles = new GameObject[editorManager.nbShapes];
		for (int i=0; i<exempleTiles.Length; i++) {
			Vector3 thisPos = new Vector3(this.transform.position.x - 20.0f + (i%2) * 40.0f,this.transform.position.y - Mathf.Floor((float)i/2) * 40.0f + 100.0f, this.transform.position.z - 0.1f);
			exempleTiles[i] = Instantiate(manager.tileModels[manager.currentType], thisPos, Quaternion.identity) as GameObject;
			exempleTiles[i].GetComponent<tile>().setShape(i);
			exempleTiles[i].GetComponent<tile>().setRotation(manager.currentRotation);
			exempleTiles[i].transform.localScale = new Vector3(30.0f,30.0f,1.0f);
			exempleTiles[i].transform.parent = this.gameObject.transform;
			if (i==manager.currentShape) {
				exempleTiles[i].GetComponent<RageSpline>().outline = RageSpline.Outline.Loop;
				exempleTiles[i].GetComponent<RageSpline>().outlineColor1 = Color.red;
				exempleTiles[i].GetComponent<RageSpline>().outlineColor2 = Color.red;
			}
		}
	}

}
