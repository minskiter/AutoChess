using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(ZST_SnapToGrid))]
public class ZST_SmartTile : MonoBehaviour {

    private const int kSideLengthInPixelsExpected = 100;

	// sprite properties get passed down
	public Color color = Color.white;
	public int orderInLayer;

    // the length in pixels refers only to the active area of the sprite
    public int sideLengthInPixels = kSideLengthInPixelsExpected;

	// tiles have tags so that one scene can have multiple tile types
	public string tileTag;

	// allowing multiple sprites allows for higher diversity of sliced tiles
	public Sprite[] sprites_center;
	public Sprite[] sprites_side;
	public Sprite[] sprites_cornerConvex;
	public Sprite[] sprites_cornerConcave;

	private Sprite[] sprites_center_Prev;
	private Sprite[] sprites_side_Prev;
	private Sprite[] sprites_cornerConvex_Prev;
	private Sprite[] sprites_cornerConcave_Prev;

	// we use coords to map tiles to row/col space
	public struct Coord {
		public Coord(int _row, int _col) {
			row = _row;
			col = _col;
		}
		public int row;
		public int col;
		public Vector2 ToVector2() {
			Vector2 vector2 = new Vector2(col,row);
			return vector2;
		}
	}

	// rerolling lets us diversify our sprites, even after they're set
	private bool doReroll;

	// we mark tiles to prevent multi-updating on a single drag of a group of tiles
	private bool midUpdate;

	// by default, we want to hide the children, but we want to allow folks to mod them inb
	private bool areChildrenHidden;

	// tiles only really make sense if they snap to a grid; plus we need this for positioning quadrants
	private ZST_SnapToGrid snap;

	private Vector2 positionPrev;

	public enum CardinalDirection {
		E, NE, N, NW, W, SW, S, SE,
	}

	// we hard-code rather than generate them
	public static Dictionary<CardinalDirection,Coord> dirsToCoordOffsets = new Dictionary<CardinalDirection, Coord>() {

		{CardinalDirection.E, new Coord(0,1)},
		{CardinalDirection.NE, new Coord(1,1)},
		{CardinalDirection.N, new Coord(1,0)},
		{CardinalDirection.NW, new Coord(1,-1)},
		{CardinalDirection.W, new Coord(0,-1)},
		{CardinalDirection.SW, new Coord(-1,-1)},
		{CardinalDirection.S, new Coord(-1,0)},
		{CardinalDirection.SE, new Coord(-1,1)},

	};

	// we only want sprites to go at certain positions radially
	private static CardinalDirection[] spriteDirections = {
		CardinalDirection.NE,
		CardinalDirection.NW,
		CardinalDirection.SW,
		CardinalDirection.SE,
	};

	// we serialize this so that we can save on a per-tile basis
	[System.Serializable]
	private class Quadrant {
		public CardinalDirection dir;
		public float startRot;
		public Vector2 offset;
		public SpriteRenderer sr;
		public Sprite rolledSprite_center;
		public Sprite rolledSprite_side;
		public Sprite rolledSprite_cornerConvex;
		public Sprite rolledSprite_cornerConcave;
	}
	[SerializeField]
	private Quadrant[] quadrants;

	// which neighbors the tile has affect how it's drawn
	public Dictionary<CardinalDirection,bool> hasNeighbor = new Dictionary<CardinalDirection, bool>();

	void Start() {
		
        SetSnap();

		// we need to set up our quadrants, if they're not already set up
		if (quadrants == null || quadrants.Length <= 0) {

			List<Quadrant> quadrantsMut = new List<Quadrant>();

			int numDirections = spriteDirections.Length;

			for (int i = 0; i < numDirections; ++i) {

				GameObject spriteObj = new GameObject();

				SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();

				float rot = (float)i/(float)numDirections * 360;
				float theta =  rot * Mathf.Deg2Rad + Mathf.PI * 0.25f;

				Vector2 offset = (new Vector2(Mathf.Sign(Mathf.Cos(theta)),Mathf.Sign(Mathf.Sin(theta)))) * 0.5f;

				CardinalDirection dir = (CardinalDirection)spriteDirections[i];
				spriteObj.name = "Sprite " + dir.ToString();
				spriteObj.transform.SetParent(transform);

				Quadrant quadrant = new Quadrant();
				quadrant.startRot = rot;
				quadrant.dir = dir;
				quadrant.offset = offset;
				quadrant.sr = sr;

				quadrantsMut.Add(quadrant);
			}

			quadrants = quadrantsMut.ToArray();

			// we want to be sure to save our serialized quadrants
			#if UNITY_EDITOR
			if (!Application.isPlaying) {
				EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
			}
			#endif
		}

		// Hide the children! Quick! Before they find out!
		HideChildren();

		// we add tiles so that the map manager knows about them
		ZST_MapManager.SharedInstance.AddTile(this);

		// we want to adjust everything when this is added so that it fits in place
		ZST_MapManager.SharedInstance.AddUpdateTag(tileTag);
	}

	void Update() {

		// for now, we don't want to update sprites during runtime; the map manager's also got a similar block up; YAY REDUNDANCY :D
		if (Application.isPlaying) {return;}

		// we call this in case we have any tags to update
		ZST_MapManager.SharedInstance.Update();

        // grid snaps are automatically set based off pixel size now
        snap.gridSideLength = (float)sideLengthInPixels / kSideLengthInPixelsExpected;

		// we should only update if we've moved significantly
		Vector2 positionCurr = snap.GetSnappedPosition(transform.position);
		float threshold = snap.gridSideLength;
		Vector2 positionDelta = positionCurr - positionPrev;
		bool didMoveSignificantly = (
			Mathf.Abs(positionDelta.x) >= threshold ||
			Mathf.Abs(positionDelta.y) >= threshold
		);
		positionPrev = positionCurr;

		// also, it matters whether or not some other thing is updating (i.e. midUpdate)
		bool doUpdate = didMoveSignificantly && !midUpdate;

		// we can safely say that we're not mid-update at this point, since we've already used that data
		midUpdate = false;

		// adding the tag tells the map manager 'update anything with this tag'
		if (doUpdate) {ZST_MapManager.SharedInstance.AddUpdateTag(tileTag);}

		// we AGGRESSIVELY set the position of the quadrants so that the user cannot accidentally move them
		float gridSideLengthCurr = snap.gridSideLength;
		foreach (Quadrant quadrant in quadrants) {
			quadrant.sr.transform.localPosition = quadrant.offset * gridSideLengthCurr * 0.5f;
			quadrant.sr.transform.localScale = Vector2.one * 0.5f;
		}

		// also, in the case that our sprite inputs have changed, we should update our sprites
		bool hasInputSpriteDataChanged = (
			sprites_center != sprites_center_Prev ||
			sprites_side != sprites_side_Prev ||
			sprites_cornerConvex != sprites_cornerConvex_Prev ||
			sprites_cornerConcave != sprites_cornerConcave_Prev
		);
		if (hasInputSpriteDataChanged) {UpdateSprite();}
		sprites_center_Prev = sprites_center;
		sprites_side_Prev = sprites_side;
		sprites_cornerConvex_Prev = sprites_cornerConvex;
		sprites_cornerConcave_Prev = sprites_cornerConcave;

		// aaaand let's apply those Sprite properties to our children!
		if (quadrants != null) {
			foreach (Quadrant quadrant in quadrants) {
				SpriteRenderer sr = quadrant.sr;
				sr.sortingOrder = orderInLayer;
				sr.color = color;
			}
		}

	}

	void OnDrawGizmos() {

		// we use a gizmo just for the clickbox, which is why it's clear
		Gizmos.color = Color.clear;
        SetSnap();
		float gridSideLength = snap.gridSideLength;
		Gizmos.DrawCube(transform.position,transform.localScale * gridSideLength);
	}

	void OnDestroy() {

		// we want to kill the children so that we don't overpopulate once we lose references
		if (!Application.isPlaying) {
			foreach (Quadrant quadrant in quadrants) {
				DestroyImmediate(quadrant.sr.gameObject);
			}
		}

		// the tile should be romeved from the map so that future updates don't include it
		ZST_MapManager.SharedInstance.RemoveTile(this);

		// we also have to update the map, since neighbors could be affected
		ZST_MapManager.SharedInstance.AddUpdateTag(tileTag);
	}

	private bool IsUnrolled() {

		// we'll say we're unrolled if ANY of our quadrants have null rolled sprites
		if (quadrants != null) {
			foreach (Quadrant quadrant in quadrants) {

				if (quadrant.rolledSprite_center == null ||
				    quadrant.rolledSprite_side == null ||
				    quadrant.rolledSprite_cornerConvex == null ||
				    quadrant.rolledSprite_cornerConcave == null) {
					return true;
				}
			}
			return false;
		}
		return true;
			
	}

	private Sprite RolledSprite(Sprite[] sprites) {
		if (sprites == null || sprites.Length <= 0) {return null;}
		return sprites[(int)(Random.value * sprites.Length)];
	}

	private bool HasNeighbor(CardinalDirection dir) {
		if (!hasNeighbor.ContainsKey(dir)) {return false;}
		return hasNeighbor[dir];
	}

	public void UpdateSprite() {

		// before updating, we'll need to roll sprites, if we haven't already
		if (doReroll || IsUnrolled()) {

			foreach (Quadrant quadrant in quadrants) {
				quadrant.rolledSprite_center = RolledSprite(sprites_center);
				quadrant.rolledSprite_side = RolledSprite(sprites_side);
				quadrant.rolledSprite_cornerConvex = RolledSprite(sprites_cornerConvex);
				quadrant.rolledSprite_cornerConcave = RolledSprite(sprites_cornerConcave);
			}

			doReroll = false;
		}

		// we've listed our enums in order, so that we can just add or subtract to get the next around the cycle
		System.Array dirs = System.Enum.GetValues(typeof(CardinalDirection));
		CardinalDirection first = (CardinalDirection)dirs.GetValue(0);
		CardinalDirection last = (CardinalDirection)dirs.GetValue(dirs.Length - 1);

		// each SpriteRenderer's gonna have a different rotation/Sprite, depending on which neighbors exist
		foreach (Quadrant quadrant in quadrants) {

			Sprite sprite = null;

			CardinalDirection dir = quadrant.dir;
			CardinalDirection dirCCW = dir.Equals(last) ? first : dir + 1;
			CardinalDirection dirCW = dir.Equals(first) ? last : dir - 1;

			float rot = quadrant.startRot;
			if (HasNeighbor(dirCW) && HasNeighbor(dirCCW)) {

				if (HasNeighbor(dir)) {
					sprite = quadrant.rolledSprite_center;
					rot = 0f; 
				} else {
					sprite = quadrant.rolledSprite_cornerConcave;
				}

			}
			else if (!HasNeighbor(dirCW) && !HasNeighbor(dirCCW)) {
				sprite = quadrant.rolledSprite_cornerConvex;
			}
			else {

				sprite = quadrant.rolledSprite_side;

				if (HasNeighbor(dirCW)) {
					rot += 90;
				}
			}

			quadrant.sr.sprite = sprite;
			quadrant.sr.transform.rotation = Quaternion.Euler(0,0,rot);
		}

	}

	public Coord GetCoord() {
        if (!snap) {SetSnap();}
		Vector2 gridPos = snap.gridSideLength == 0 ? Vector2.zero : (Vector2)snap.GetGridPosition()/snap.gridSideLength;
		Coord coord = new Coord((int)gridPos.y,(int)gridPos.x);
		return coord;
	}

	public void MarkAsMidUpdate() {
		midUpdate = true;
	}

	public void RerollSprite() {
		doReroll = true;
		UpdateSprite();
	}

	public void HideOrShowChildren() {
		areChildrenHidden = !areChildrenHidden;
		HideOrShowChildrenHelper();
	}

	public void HideChildren() {
		areChildrenHidden = true;
		HideOrShowChildrenHelper();
	}

	private void HideOrShowChildrenHelper() {
		foreach (Quadrant quadrant in quadrants) {
			quadrant.sr.gameObject.hideFlags = areChildrenHidden ? HideFlags.HideInHierarchy : HideFlags.None;
		}
	}

	public bool AreChildrenHidden() {
		return areChildrenHidden;
	}

    private void SetSnap() {
        if (snap != null) {return;}
        snap = GetComponent<ZST_SnapToGrid>();
        snap.hideFlags = HideFlags.HideInInspector;
    }
		
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ZST_SmartTile))]
public class ZST_SmartTileEditor : Editor {

	private Vector3 positionPrev;

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		if (GUILayout.Button("Reroll Sprite")) {
			foreach (Object targ in targets) {
				ZST_SmartTile script = (ZST_SmartTile)targ;
				if (!script) {continue;}

				script.RerollSprite();

				if (!Application.isPlaying) {
					EditorUtility.SetDirty(script);
				}

			}
		}

		ZST_SmartTile smartSprite = (ZST_SmartTile)target;
		string hideOrShowPrefix = smartSprite.AreChildrenHidden() ? "Show" : "Hide";
		if (GUILayout.Button(hideOrShowPrefix + " Children")) {
			smartSprite.HideOrShowChildren();
		}

		if (!Application.isPlaying) {
			EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}

	}
}
#endif
