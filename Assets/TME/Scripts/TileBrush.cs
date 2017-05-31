using UnityEngine;
using System.Collections;

namespace TME
{
	public class TileBrush : MonoBehaviour
	{
		private Vector2 brushSize = Vector2.zero;

		public Vector2 BrushSize { set { this.brushSize = value; } }

		private int tileID = 0;

		public int TileID {
			get {
				return this.tileID;
			}
			set {
				tileID = value;
			}
		}

		private SpriteRenderer renderer2D;

		public SpriteRenderer Renderer2D { get { return this.renderer2D; } set { this.renderer2D = value; } }

		public bool AddCollider2D {
			get;
			set;
		}

		void OnDrawGizmoSelected ()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube (transform.position, brushSize);

		}

		public void UpdateBrush (Sprite sprite)
		{
			renderer2D.sprite = sprite;
		}
	}
}
