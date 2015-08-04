using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TheCubers
{
	public class UILeaderboards : UIMenu
	{
		private class itemUI
		{
			public Text Name;
			public Text Score;

			public itemUI(GameObject obj)
			{
				obj.transform.localScale = Vector3.one;
				var txt = obj.GetComponentsInChildren<Text>(true);
				if (txt == null || txt.Length != 2)
					Debug.LogError("Score item object layout is not correct!");
				Name = txt[0];
				Score = txt[1];
			}
		}

		public Text Title;

		public Transform ItemParent;
		public GameObject FirstItem;
		public int Count = 5;

		private itemUI[] items;


		protected override void OnInit()
		{
			items = new itemUI[Count];

			items[0] = new itemUI(FirstItem);
			for (int i = 1; i < items.Length; ++i)
			{
				GameObject obj = Instantiate(FirstItem);
				obj.transform.SetParent(ItemParent);
				items[i] = new itemUI(obj);
			}
		}

		protected override void OnOpenStart()
		{
			var menu = UIBase.Instance.GetMenu<UILevelInfo>();
			string last = menu.Last;

			var levels = MyFiles.LoadLevelScores();
			if (levels != null && levels.Count > 0)
			{
				for (int i = 0; i < levels.Count; ++i)
					if (levels[i].Name == last)
						populate(levels[i]);
			}
			else
			{
				populate(new LevelScores());
			}
		}


		public void AddScore(int score)
		{
			string last = UIBase.Instance.GetMenu<UILevelInfo>().Last;
			string name = UIBase.Instance.GetMenu<UIProfiles>().CurrentName;

			// try open file
			var levels = MyFiles.LoadLevelScores();
			if (levels != null)
			{
				bool added = false;
				// find 
				for (int i = 0; i < levels.Count; ++i)
				{
					if (levels[i].Name == last)
					{
						for (int j = 0; j < levels[i].Names.Count; ++j)
						{
							if (levels[i].Scores[j] < score)
							{
								levels[i].Names.Insert(j, name);
								levels[i].Scores.Insert(j, score);
								added = true;
								break;
							}
						}
						if (!added)
						{
							levels[i].Names.Add(name);
							levels[i].Scores.Add(score);
							added = true;
						}
						break;
					}
				}
				if (!added)
				{
					levels.Add(new LevelScores()
					{
						Name = last,
						Names = new List<string>(new string[] { name }),
						Scores = new List<int>(new int[] { score })
					});
				}
			}
			else
			{
				levels = new List<LevelScores>();
				levels.Add(new LevelScores()
				{
					Name = last,
					Names = new List<string>(new string[] { name }),
					Scores = new List<int>(new int[] { score })
				});
			}

			MyFiles.SaveLevelScores(levels);
		}

		private void populate(LevelScores level)
		{
			for (int i = 0; i < items.Length; ++i)
			{
				if (level.Names != null && i < level.Names.Count)
				{
					items[i].Name.text = string.Format("{0,2}: {1}", i + 1, level.Names[i]);
					items[i].Score.text = level.Scores[i].ToString("N0");
				}
				else
				{
					items[i].Name.text = string.Format("{0,2}: {1}", i + 1, "None");
					items[i].Score.text = 0.ToString("N0");
				}
			}
		}
	}
}
