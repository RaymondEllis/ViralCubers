using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UILevelInfo : UIMenu
	{
		public struct Info
		{
			public string Level, Title, Description;
			public int GoalType;
			public int GoalScore;

			public static Info[] Get()
			{
				var infoFile = Resources.Load<TextAsset>("level info");
				return LitJson.JsonMapper.ToObject<Info[]>(infoFile.text);
			}
		}

		public Text Title;
		public Text Text;

		// the last known level.
		public string Last { get; private set; }

		public void Play()
		{
			UIBase.Instance.LoadLevel(Last);
		}

		public void GoMe(string level)
		{
			Last = level;

			var infos = Info.Get();

			Info info = new Info();
			for (int i = 0; i < infos.Length; ++i)
			{
				if (infos[i].Level == Last)
				{
					info = infos[i];
					break;
				}
			}

			Title.text = info.Title;
			Text.text = info.Description;

			//string name = "";
			//string text = "";

			//switch (level)
			//{
			//	case "world":
			//		name = "World";
			//		text = "Level to test stuffs.";
			//		break;
			//	case "level1":
			//		name = "Level 1";
			//		text = "A beginer level.";
			//		break;
			//}

			//Title.text = name;
			//Text.text = text;
			UIBase.Instance.Go(this);
		}
	}
}
