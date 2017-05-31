using UnityEngine;
using System.Collections;
using UnityEditor;
using TME;

[CustomEditor (typeof(TileMap))]
public class E_TileMapEditor : Editor
{
	private static readonly string MAP_SIZE_LBL = "Map Size";
	private static readonly string SPRITESHEET_LBL = "Spritesheet: ";
	private static readonly string NULL_TEXTURE2D_WARNING = "No Texture2D selected";
	private static readonly string TILE_SIZE_LBL = "Tile Size:";
	private static readonly string GRID_SIZE_LBL = "Grid Size:";
	private static readonly string PXLS_TO_UNITS_LBL = "Pixels To Units:";

	private TileMap map;
	private TileBrush brush;
	private Vector3 mouseHitPos;

	private bool mouseOnMap {
		get {
			return mouseHitPos.x > 0 && mouseHitPos.x < map.gridSize.x
				&& mouseHitPos.y < 0 && mouseHitPos.y > -map.gridSize.y;
				
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.BeginVertical ();

		var oldMapSize = map.mapSize;
		Vector2 mapSize = EditorGUILayout.Vector2Field (MAP_SIZE_LBL, map.mapSize);
		

		var oldTexture = map.mapTexture;
		Texture2D mapTexture = (Texture2D)EditorGUILayout.ObjectField (SPRITESHEET_LBL, map.mapTexture, typeof(Texture2D), false);	


		if (EditorGUI.EndChangeCheck ()) {

			bool updateParams = false;

			if (mapSize != oldMapSize) {
				Undo.RecordObject (target, "Changed Map Size");

				map.mapSize = mapSize;

				updateParams = true;
			}
				
			if (mapTexture != oldTexture) {
				Undo.RecordObject (target, "Changed Map Texture");

				map.mapTexture = mapTexture;
				map.TileID = 1;
				CreateBrush ();

				updateParams = true;
			}

			if (updateParams) {
				InjectParameters ();
			}
		}

		if (map.mapTexture == null) {
			EditorGUILayout.HelpBox (NULL_TEXTURE2D_WARNING, MessageType.Warning);
		} else {
			EditorGUILayout.LabelField (TILE_SIZE_LBL, map.tileSize.x + "x" + map.tileSize.y);
			EditorGUILayout.LabelField (GRID_SIZE_LBL, map.gridSize.x + "x" + map.gridSize.y);
			EditorGUILayout.LabelField (PXLS_TO_UNITS_LBL, map.pixelsToUnits.ToString ());

			if (GUILayout.Button ("Clear Tiles")) {
				if (EditorUtility.DisplayDialog ("Clear map's tiles?", "Are you sure?", "Yes", "No")) {
					ClearMap ();
				}
			}
		}

		EditorGUILayout.EndVertical ();
	}

	void OnSceneGUI ()
	{
		if (brush != null) {
			UpdateHitPosition ();
			MoveBrush ();

			if (map.TileBrushUpdated) {
				UpdateBrush (map.CurrentTileBrush);
			}
		}

		if (map.mapTexture != null && mouseOnMap) {
			Event current = Event.current;
			if (current.shift) {
				Draw ();
			} else if (current.alt) {
				RemoveTile ();
			}
		}
	}

	public void UpdateBrush (Sprite sprite)
	{
		if (brush == null && map.mapTexture != null) {
			CreateBrush ();
		}
		
		if (brush != null && map.mapTexture != null) {
			brush.UpdateBrush (sprite);
		}
	}

	//TODO: inject new data on property change in inspector
	void OnEnable ()
	{
		map = target as TileMap;
		Tools.current = Tool.View;

		if (map.Tiles == null) {

			bool goFound = false;

			foreach (Transform t in map.transform) {
				if (t.gameObject.name.Equals ("Tiles")) {
					map.Tiles = t.gameObject;
					goFound = true;
					break;
				}
			}

			if (!goFound) {
				var go = new GameObject ("Tiles");
				go.transform.SetParent (map.transform);
				go.transform.position = Vector3.zero;
				map.Tiles = go;
			}

		
		}

		InjectParameters ();

		NewBrush ();
	}

	void OnDisable ()
	{
		DestroyBrush ();
	}

	private void Draw ()
	{
		var id = brush.TileID.ToString ();

		var posX = brush.transform.position.x;
		var posY = brush.transform.position.y;

		GameObject tile = GameObject.Find (map.name + "/Tiles/tile_" + id);

		if (tile == null) {

			tile = new GameObject ("tile_" + id);
			tile.transform.SetParent (map.Tiles.transform);
			tile.transform.position = new Vector3 (posX, posY, 0);
			tile.AddComponent<SpriteRenderer> ();

			// add additional components here
			if (brush.AddCollider2D) {
				var collider2D = tile.AddComponent<BoxCollider2D> ();
				collider2D.size = new Vector2 (map.tileSize.x / map.pixelsToUnits, map.tileSize.y / map.pixelsToUnits);
			}
		}

		tile.GetComponent<SpriteRenderer> ().sprite = brush.Renderer2D.sprite;
	}

	private void RemoveTile ()
	{
		var id = brush.TileID.ToString ();
		GameObject tile = GameObject.Find (map.name + "/Tiles/tile_" + id);

		if (tile != null) {
			DestroyImmediate (tile);
		}

	}

	private void ClearMap ()
	{
		for (var i = 0; i < map.Tiles.transform.childCount; i++) {
			var t = map.Tiles.transform.GetChild (i);

			DestroyImmediate (t.gameObject);
			i--;

		}
	}


	private void InjectParameters ()
	{
		if (map.mapTexture == null) {
			return;
		}

		map.spriteRefs = AssetDatabase.LoadAllAssetsAtPath (AssetDatabase.GetAssetPath (map.mapTexture));

		if (map.spriteRefs.Length > 1) {
			// first item of array is always reference to parent texture2d.
			var sprite = (Sprite)map.spriteRefs [1];
			var width = sprite.textureRect.width;
			var height = sprite.textureRect.height;

			map.tileSize = new Vector2 (width, height);

			map.pixelsToUnits = (int)(sprite.rect.width / sprite.bounds.size.x);

			map.gridSize = new Vector2 ((map.tileSize.x / map.pixelsToUnits) * map.mapSize.x, 
				(map.tileSize.y / map.pixelsToUnits) * map.mapSize.y);
		}

	}

	private void CreateBrush ()
	{

		if (map.mapTexture == null) {
			return;
		}
			
		var sprite = map.CurrentTileBrush;

		if (sprite != null) {
			bool brushFound = false;

			foreach (Transform t in map.transform) {
				if (t.gameObject.name.Equals ("Brush")) {
					brush = t.gameObject.GetComponent<TileBrush> ();
					brush.Renderer2D = t.gameObject.GetComponent<SpriteRenderer> ();
					brushFound = true;
					break;
				}
			}

			if (!brushFound) {
				GameObject go = new GameObject ("Brush");
				go.transform.SetParent (map.transform);

				brush = go.AddComponent<TileBrush> ();
				brush.Renderer2D = go.AddComponent<SpriteRenderer> ();
				brush.Renderer2D.sortingOrder = 1000;
			}

		

			var pixelToUnits = map.pixelsToUnits;

			brush.BrushSize = new Vector2 (sprite.textureRect.width / pixelToUnits,
				sprite.textureRect.height / pixelToUnits);
			brush.UpdateBrush (sprite);
		}
	}

	private void NewBrush ()
	{
		if (brush == null) {
			CreateBrush ();
		}
	}

	private void DestroyBrush ()
	{
		if (brush != null) {
			DestroyImmediate (brush.gameObject);
		}
	}

	private void UpdateHitPosition ()
	{
		var p = new Plane (map.transform.TransformDirection (Vector3.forward), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		var hit = Vector3.zero;
		float dist = 0f;

		if (p.Raycast (ray, out dist)) {
			hit = ray.origin + ray.direction.normalized * dist;
		}

		mouseHitPos = map.transform.InverseTransformPoint (hit);
	}

	private void MoveBrush ()
	{
		var tileSize = map.tileSize.x / map.pixelsToUnits;

		var x = Mathf.Floor (mouseHitPos.x / tileSize) * tileSize;
		var y = Mathf.Floor (mouseHitPos.y / tileSize) * tileSize;

		var row = x / tileSize;
		var col = Mathf.Abs (y / tileSize) - 1;

		if (!mouseOnMap) {
			return;
		}

		var id = (int)((col * map.mapSize.x) + row);

		brush.TileID = id;

		x += map.transform.position.x + tileSize / 2;
		y += map.transform.position.y + tileSize / 2;


		brush.transform.position = new Vector3 (x, y, map.transform.position.z);
	}
}
