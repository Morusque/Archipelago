using UnityEngine;
using System.Collections;
using System.Xml;

public class editorManager : MonoBehaviour {

	public const int nbTilesX = 20;
	public const int nbTilesY = 15;
	public const int nbLayers = 3;
	public const int nbScreensX = 20;
	public const int nbScreensY = 20;
	public const int nbShapes = 6;

	public Vector2 targetResolution = new Vector2 (800, 600);

	public Camera cam;

	public GameObject[] tileModels = new GameObject[2];
	public int currentType=1;
	public int currentShape=0;
	public int currentRotation=3;
	int currentLayer=0;

	public GameObject[] gradients = new GameObject[2];
	public GameObject levelLayer;
	public GameObject[] levelLayers = new GameObject[3];

	public GameObject uiVertical;

	public Vector2 levelBL;
	public Vector2 levelTR;

	GameObject pointerObject;
	Vector2 selTilePos = new Vector2(0,0);

	public ScreenInfos[,] screens = new ScreenInfos[nbScreensX,nbScreensY];
	int screenPosX = 0;
	int screenPosY = 0;
	public GameObject[,,] tiles = new GameObject[nbTilesX,nbTilesY,nbLayers];

	void Start () {
		importLevelDesign();
		uiVertical.GetComponent<uiUpdater>().updateUi(this);
	}

	void Update () {

		// find current pointed tile
		updatePointerObject();

		// left/right = select type
		if (Input.GetKeyDown("left")||Input.GetKeyDown("right")) {
			currentType=(currentType+tileModels.Length+(Input.GetKeyDown("right")?1:-1))%tileModels.Length;
			updatePointerObject();
			uiVertical.GetComponent<uiUpdater>().updateUi(this);
		}

		// mouse wheel = select shape
		if (Input.GetAxis("Mouse ScrollWheel")!=0) {
			currentShape=(currentShape+(Input.GetAxis("Mouse ScrollWheel")>0?1:-1)+nbShapes)%nbShapes;
			uiVertical.GetComponent<uiUpdater>().updateUi(this);
		}

		// right click = rotate
		if (Input.GetMouseButtonDown (1)) {
			currentRotation = (currentRotation + 1) % 4;
			pointerObject.GetComponent<tile> ().setRotation (currentRotation);
			uiVertical.GetComponent<uiUpdater>().updateUi(this);
		}

		// up/down = select layer
		if (Input.GetKeyDown("down")||Input.GetKeyDown("up")) {
			if(Input.GetKeyDown("up")) currentLayer = (currentLayer+1)%nbLayers;
			if(Input.GetKeyDown("down")) currentLayer = (currentLayer-1+nbLayers)%nbLayers;
			cam.transform.position = new Vector3(cam.transform.position.x,cam.transform.position.y,currentLayer*2.0f-0.5f);
		}

		// left click = place tile
		if(Input.GetMouseButton(0)) {
			if (selTilePos.x>=0&&selTilePos.x<nbTilesX&&selTilePos.y>=0&&selTilePos.y<nbTilesY) {
				Destroy(tiles[(int)selTilePos.x,(int)selTilePos.y,currentLayer]);
				if (Input.GetMouseButton(0)) {
					setTile((int)selTilePos.x,(int)selTilePos.y,currentLayer,currentType,currentShape,currentRotation);
				}
			}
		}

		// c = random colors
		if (Input.GetKeyDown("c")) {
			cam.backgroundColor = new Color(Random.value,Random.value,Random.value);
			for (int i=0;i<gradients.Length;i++) gradients[i].GetComponent<gradient>().updateColors();
		}

		// e = export
		if (Input.GetKeyDown ("e")) exportLevelDesign();

		// i = export
		if (Input.GetKeyDown ("i")) importLevelDesign();

		// numpad = select screen
		if (Input.GetKeyDown ("[6]") || Input.GetKeyDown ("[4]") || Input.GetKeyDown ("[8]") || Input.GetKeyDown ("[2]")) {
			storeCurrentScreen ();
			if (Input.GetKeyDown ("[6]"))
				screenPosX = (screenPosX + 1) % screens.GetLength (0);
			if (Input.GetKeyDown ("[4]"))
				screenPosX = (screenPosX - 1 + screens.GetLength (0)) % screens.GetLength (0);
			if (Input.GetKeyDown ("[8]"))
				screenPosY = (screenPosY + 1) % screens.GetLength (1);
			if (Input.GetKeyDown ("[2]"))
				screenPosY = (screenPosY - 1 + screens.GetLength (1)) % screens.GetLength (1);
			updateDisplayedLevel();
		}

	}

	void importLevelDesign() {
		string filepath = Application.dataPath + @"/levels/Level.xml";
		if (System.IO.File.Exists(filepath)) {
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(System.IO.File.ReadAllText(filepath));
			foreach (XmlElement screen in xmlDoc.GetElementsByTagName("screen")) {
				if (screen.GetAttribute("empty")=="0") {
					int scX = int.Parse(screen.GetAttribute("x"));
					int scY = int.Parse(screen.GetAttribute("y"));
					if (screens[scX,scY]==null) screens[scX,scY] = new ScreenInfos();
					foreach (XmlElement color in screen.GetElementsByTagName("color")) {
						screens[scX,scY].colors[int.Parse(color.GetAttribute("i"))] = 
							new Color(float.Parse(color.GetAttribute("r")),float.Parse(color.GetAttribute("g")),float.Parse(color.GetAttribute("b")));
					}
					foreach (XmlElement tile in screen.GetElementsByTagName("tile")) {
						screens[scX,scY].tiles[int.Parse(tile.GetAttribute("x")),int.Parse(tile.GetAttribute("y")),int.Parse(tile.GetAttribute("l"))] = 
							new TileInfos(int.Parse(tile.GetAttribute("t")),int.Parse(tile.GetAttribute("s")),int.Parse(tile.GetAttribute("r")));
					}
				}
			}
		}
		updateDisplayedLevel();
	}

	void setTile(int x, int y, int l, int t, int s, int r) {
		if (tiles[x,y,l]!=null) Destroy(tiles[x,y,l]);
		float xPos = (x + 0.5f);
		float yPos = (y + 0.5f);
		xPos *= (levelTR.x - levelBL.x)/(float)nbTilesX;
		yPos *= (levelTR.y - levelBL.y)/(float)nbTilesY;
		xPos += levelBL.x;
		yPos += levelBL.y;
		Vector3 position = new Vector3(xPos, yPos, (l*2)+1);
		tiles[x,y,l] = Instantiate(tileModels[t],position,Quaternion.identity) as GameObject;
		tiles[x,y,l].transform.localScale = new Vector3((levelTR.x - levelBL.x)/(float)nbTilesX,(levelTR.y - levelBL.y)/(float)nbTilesY,1);
		tiles[x,y,l].transform.parent = levelLayers[l].transform;
		tiles[x,y,l].GetComponent<tile>().setShape(s);
		tiles[x,y,l].GetComponent<tile>().setRotation(r);
	}

	void exportLevelDesign() {
		storeCurrentScreen ();
		string filepath = Application.dataPath + @"/levels/Level.xml";
		XmlDocument xmlDoc = new XmlDocument();
		XmlElement root = xmlDoc.CreateElement("level");
		xmlDoc.AppendChild(root);
		XmlElement infos = xmlDoc.CreateElement("infos");
		infos.SetAttribute("version","1");
		root.AppendChild(infos);
		for (int scX=0;scX<20;scX++) {
			for (int scY=0;scY<20;scY++) {
				XmlElement thisScreen = xmlDoc.CreateElement("screen");
				thisScreen.SetAttribute("x",scX.ToString());
				thisScreen.SetAttribute("y",scY.ToString());
				if (screens[scX,scY]==null) {
					thisScreen.SetAttribute("empty","1");
				} else {
					// TODO put all these things in the "infos" classes
					thisScreen.SetAttribute("empty","0");
					for (int colorI=0 ; colorI<screens[scX,scY].colors.Length ; colorI++) {
						XmlElement thisColor = xmlDoc.CreateElement("color");
						thisColor.SetAttribute("i",colorI.ToString());
						thisColor.SetAttribute("r",screens[scX,scY].colors[colorI].r.ToString());
						thisColor.SetAttribute("g",screens[scX,scY].colors[colorI].g.ToString());
						thisColor.SetAttribute("b",screens[scX,scY].colors[colorI].b.ToString());
						thisScreen.AppendChild(thisColor);
					}
					for (int x=0;x<20;x++) {
						for (int y=0;y<15;y++) {
							for (int l=0;l<3;l++) {
								XmlElement thisTile = xmlDoc.CreateElement("tile");
								thisTile.SetAttribute("x",x.ToString());
								thisTile.SetAttribute("y",y.ToString());
								thisTile.SetAttribute("l",l.ToString());
								thisTile.SetAttribute("t",screens[scX,scY].tiles[x,y,l].type.ToString());
								thisTile.SetAttribute("s",screens[scX,scY].tiles[x,y,l].shape.ToString());
								thisTile.SetAttribute("r",screens[scX,scY].tiles[x,y,l].rotation.ToString());
								thisScreen.AppendChild(thisTile);
							}
						}
					}
				}
				root.AppendChild(thisScreen);
			}
		}
		xmlDoc.Save(filepath);
		print("export done");
	}
	
	void storeCurrentScreen() {
		if (screens [screenPosX, screenPosY] == null) {
			screens [screenPosX, screenPosY] = new ScreenInfos ();
		}
		for (int i=0;i<screens[screenPosX, screenPosY].colors.Length;i++) screens[screenPosX, screenPosY].colors[i] = cam.backgroundColor;
		for (int x=0;x<nbTilesX;x++) {
			for (int y=0;y<nbTilesY;y++) {
				for (int l=0;l<nbLayers;l++) {
					screens[screenPosX, screenPosY].tiles[x,y,l] = tiles[x,y,l].GetComponent<tile>().getTileInfos();
				}
			}
		}
	}

	void updateDisplayedLevel() {
		if (screens [screenPosX, screenPosY] == null) {
			screens [screenPosX, screenPosY] = new ScreenInfos ();
		}
		cam.backgroundColor = screens [screenPosX, screenPosY].colors[0];
		for (int i=0;i<gradients.Length;i++) gradients[i].GetComponent<gradient>().updateColors();
		for (int x=0;x<nbTilesX;x++) {
			for (int y=0;y<nbTilesY;y++) {
				for (int l=0;l<nbLayers;l++) {
					TileInfos thisTileInfos = screens[screenPosX,screenPosY].tiles[x,y,l];
					setTile(x,y,l,thisTileInfos.type,thisTileInfos.shape,thisTileInfos.rotation);
				}
			}
		}
	}

	void updatePointerObject() {
		Vector2 mouseInFrame = new Vector2 (Input.mousePosition.x*targetResolution.x /Screen.width,
		                                    Input.mousePosition.y*targetResolution.y/Screen.height);
		Vector2 mouseInScreen = new Vector2((mouseInFrame.x-levelBL.x)*nbTilesX/(levelTR.x-levelBL.x),
		                                    (mouseInFrame.y-levelBL.y)*nbTilesY/(levelTR.y-levelBL.y));
		selTilePos = new Vector2(Mathf.Floor(mouseInScreen.x),Mathf.Floor(mouseInScreen.y));
		Vector3 mouseInEditor = new Vector3((selTilePos.x+0.5f)*(levelTR.x-levelBL.x)/nbTilesX+levelBL.x,
		                                    (selTilePos.y+0.5f)*(levelTR.y-levelBL.y)/nbTilesY+levelBL.y,
		                                    cam.transform.position.z+1.0f);
		Destroy(pointerObject);
		if (selTilePos.x>=0&&selTilePos.y>=0&&selTilePos.x<nbTilesX&&selTilePos.y<nbTilesY) {
			pointerObject = Instantiate (tileModels[currentType], mouseInEditor, Quaternion.identity) as GameObject;
			pointerObject.transform.localScale = new Vector3((levelTR.x - levelBL.x)/(float)nbTilesX,(levelTR.y - levelBL.y)/(float)nbTilesY,1);
			pointerObject.GetComponent<tile>().setShape(currentShape);
			pointerObject.GetComponent<tile>().setRotation(currentRotation);
		}
	}
	
}

public class ScreenInfos {
	public TileInfos[,,] tiles = new TileInfos[editorManager.nbTilesX,editorManager.nbTilesY,editorManager.nbLayers];// [x,y,layer]
	public Color[] colors = new Color[1];// background

	public ScreenInfos() {
		for (int i=0;i<colors.Length;i++) colors[i] = new Color(0.9f,0.5f,0.8f);
		for (int x=0;x<editorManager.nbTilesX;x++) { 
			for (int y=0;y<editorManager.nbTilesY;y++) {
				for (int l=0;l<editorManager.nbLayers;l++) {
					tiles[x,y,l] = new TileInfos(0,0,0);
				}
			}
		}
	}
}

public class TileInfos {
	public int type = 0;
	public int shape = 0;
	public int rotation = 0;
	public TileInfos (int t, int s, int r) {
		this.type = t;
		this.shape = s;
		this.rotation = r;
	}
}

