using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Sudoku
{
	public class StatsScreen : Screen
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private StatsListItem	statsListItemPrefab	= null;
		[SerializeField] private Transform		statsListContainer	= null;
        [SerializeField] private Color[]        itemColors = new Color[4];
		#endregion

		#region Member Variables

		private ObjectPool statsListItemPool;

		#endregion

		#region Public Methods

		public override void Initialize()
		{
			base.Initialize();

			statsListItemPool = new ObjectPool(statsListItemPrefab.gameObject, 4, statsListContainer);
		}

		public override void Show(bool back, bool immediate)
		{
			base.Show(back, immediate);

			statsListItemPool.ReturnAllObjectsToPool();
            statsListContainer.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			for (int i = 0; i < GameManager.Instance.PuzzleGroupDatas.Count; i++)
			{
                GameObject obj = statsListItemPool.GetObject();
                StatsListItem item = obj.GetComponent<StatsListItem>();
                item.Setup(GameManager.Instance.PuzzleGroupDatas[i]);
                item.foreground.color = itemColors[i];
				//statsListItemPool.GetObject<StatsListItem>().Setup(GameManager.Instance.PuzzleGroupDatas[i]);
			}
		}

		#endregion
	}
}
