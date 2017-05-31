using UnityEngine;
using System.Collections;
using UnityEditor;
using TME;

public class E_CreateTileMapMenu
{
	private static readonly string CONTAINER_NAME = "Tile Map Container";

	private static int CONT_COUNT = 0;

	[MenuItem ("GameObject/TME/Tile Map")]
	public static void CreateTileMap ()
	{
		var l_gameObject = new GameObject (CONTAINER_NAME + "_" + CONT_COUNT++);
		l_gameObject.AddComponent<TileMap> ();
	}
}
