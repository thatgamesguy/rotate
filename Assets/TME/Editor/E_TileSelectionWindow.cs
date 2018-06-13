using UnityEngine;
using System.Collections;
using UnityEditor;
using TME;

public class E_TileSelectionWindow : EditorWindow
{
	private static readonly string WINDOW_TITLE = "Tile Selection";
	private static readonly int SCROLLBAR_SIZE = 5;

	private enum Scale
	{
		x1,
		x2,
		x3,
		x4,
		x5
	}

	private Scale scale;

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 currentSelection = Vector2.zero;

	[MenuItem ("Window/TME/Tile Selector")]
	public static void OpenTileSelectionWindow ()
	{
		var window = EditorWindow.GetWindow (typeof(E_TileSelectionWindow));
		window.titleContent.text = WINDOW_TITLE;
	}

	void OnGUI ()
	{
		var selectedObj = Selection.activeObject;
		
		if (selectedObj == null) {
			return;
		}

		if (!(selectedObj is GameObject)) {
			return;
		}

		var selection = ((GameObject)selectedObj).GetComponent<TileMap> ();

		if (selection == null) {
			return;
		}
		 
		var texture2D = selection.mapTexture;
		if (texture2D != null) {


			scale = (Scale)EditorGUILayout.EnumPopup ("Zoom", scale);
			var newScale = ((int)scale) + 1;
			var newTextureSize = new Vector2 (texture2D.width, texture2D.height) * newScale;
			var offset = new Vector2 (10, 25);

			var viewPort = new Rect (0, 0, position.width - SCROLLBAR_SIZE, position.height - SCROLLBAR_SIZE);
			var contentSize = new Rect (0, 0, newTextureSize.x + offset.x, (newTextureSize.y + offset.y));

			scrollPosition = GUI.BeginScrollView (viewPort, scrollPosition, contentSize);


			GUI.DrawTexture (new Rect (offset.x, offset.y, newTextureSize.x, newTextureSize.y), texture2D);

			var tile = selection.tileSize * newScale;
			var grid = new Vector2 (newTextureSize.x / tile.x, newTextureSize.y / tile.y);

			var selectionPos = new Vector2 (tile.x * currentSelection.x + offset.x,
				                   tile.y * currentSelection.y + offset.y);

			var boxTex = new Texture2D (1, 1);
			boxTex.SetPixel (0, 0, new Color (0, 0.5f, 1f, 0.4f));
			boxTex.Apply ();

			var style = new GUIStyle (GUI.skin.customStyles [0]);
			style.normal.background = boxTex;

			GUI.Box (new Rect (selectionPos.x, selectionPos.y, tile.x, tile.y), "", style);

			TileBrush brush = null;

			foreach (Transform t in ((GameObject) selectedObj).transform) {
				brush = t.GetComponent<TileBrush> ();
				if (brush != null) {
					break;
				}
			}

			if (brush != null) {
				brush.AddCollider2D = GUI.Toggle (new Rect (offset.x, newTextureSize.y + (offset.y * 2), 100, 25), 
					brush.AddCollider2D, "Collider2D");
			}

			var cEvent = Event.current;
			Vector2 mousePos = new Vector2 (cEvent.mousePosition.x, cEvent.mousePosition.y);
			if (cEvent.type == EventType.MouseDown && cEvent.button == 0) {
				currentSelection.x = Mathf.Floor ((mousePos.x + scrollPosition.x) / tile.x);
				currentSelection.y = Mathf.Floor ((mousePos.y + scrollPosition.y) / tile.y);

				if (currentSelection.x > grid.x - 1)
					currentSelection.x = grid.x - 1;

				if (currentSelection.y > grid.y - 1)
					currentSelection.y = grid.y - 1;

				selection.TileID = (int)(currentSelection.x + (currentSelection.y * grid.x) + 1);

				Repaint ();
			}

			GUI.EndScrollView ();
		}
	}
}
