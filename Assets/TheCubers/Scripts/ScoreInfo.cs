using UnityEngine;

namespace TheCubers
{
	public static class ScoreInfo
	{

		public static string GoalText(string level)
		{
			var info = MyFiles.LoadLevelInfo(level);
			int cat = info.GoalType / 10;
			int sub = info.GoalType % 10;

			string extraStr = "Score";
			if (cat == 1)
				extraStr = "Cubers win with a score";
			else if (cat == 2)
				extraStr = "Virus win with a score";

			switch (sub)
			{
				case 0:
					Debug.LogError("Did not find level info? " + level);
					break;
				case 1: // more
					return string.Format("Goal: {0} more than {1:n0}", extraStr, info.GoalScore);
				case 2: // equal
					return string.Format("Goal: {0} must equal {1:n0}", extraStr, info.GoalScore);
				case 3: // less
					return string.Format("Goal: {0} less than {1:n0}", extraStr, info.GoalScore);
			}
			return "Unknown goal";
		}

		public static string FailText(string level, bool cuberWin, int score)
		{
			var info = MyFiles.LoadLevelInfo(level);
			int cat = info.GoalType / 10;
			int sub = info.GoalType % 10;

			if (cat == 1 && !cuberWin)
				return "Cubers must win!";
			else if (cat == 2 && cuberWin)
				return "Virus must win!";

			switch (sub)
			{
				case 0:
					Debug.LogError("Did not find level info? " + level);
					break;// ToDo  1: Fix format to format 1,000 proper.
				case 1: // more
					return string.Format("You needed {0:n0} or more points to finish.", info.GoalScore - score);
				case 2: // equal
					if (score > info.GoalScore)
						return string.Format("You needed exactly {0:n0} fewer points to finish.", info.GoalScore);
					else
						return string.Format("You needed exactly {0:n0} more points to finish.", info.GoalScore);
				case 3: // less
					return string.Format("You needed {0:n0} fewer points to finish.", score - info.GoalScore);
			}
			return "Unknown goal";
		}

		public static bool Test(string level, bool cuberWin, int score)
		{
			var info = MyFiles.LoadLevelInfo(level);
			int cat = info.GoalType / 10;
			int sub = info.GoalType % 10;

			// cat of 1 cubers must win
			if (cat == 1 && !cuberWin)
				return false;
			// cat of 2 virus must win
			else if (cat == 2 && cuberWin)
				return false;


			switch (sub)
			{
				case 0:
					Debug.LogError("Did not find level info? " + level);
					break;
				case 1: // more
					if (score > info.GoalScore)
						return true;
					break;
				case 2: // equal
					if (score == info.GoalScore)
						return true;
					break;
				case 3: // less
					if (score < info.GoalScore)
						return true;
					break;
			}
			return false;
		}
	}
}
