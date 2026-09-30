using UnityEngine;

namespace _Bludoku.Scripts
{
	public class ManagerTheme : MonoBehaviour
	{
		public Color bricks_color;
		public Color unavail_brick_color;
		public Color tile_destroyable_color;
		public Color grid_second_color;

		static ManagerTheme instance;
		public static ManagerTheme Instance
		{
			get
			{
				if (instance == null) instance = GameObject.Find("ManagerTheme")?.GetComponent<ManagerTheme>();
				return instance;
			}
		}

	}
}