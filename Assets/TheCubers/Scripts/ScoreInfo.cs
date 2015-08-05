using UnityEngine;

namespace TheCubers
{
	public static class ScoreInfo
	{

		public static string GoalText(string level)
		{
			var info = MyFiles.LoadLevelInfo(level);
			switch (info.GoalType)
			{
				case 0:
					Debug.LogError("Did not find level info? " + level);
					break;
				case 1: // more
					return "Goal: Score more than " + info.GoalScore.ToString("N0");
				case 2: // equal
					return "Goal: Score must equal " + info.GoalScore.ToString("N0");
				case 3: // less
					return "Goal: Score less than " + info.GoalScore.ToString("N0");
			}
			return "Unknown goal";
		}

		public static string FailText(string level, int score)
		{
			var info = MyFiles.LoadLevelInfo(level);
			switch (info.GoalType)
			{
				case 0:
					Debug.LogError("Did not find level info? " + level);
					break;
				case 1: // more
					return string.Format("You needed {0:0} or more points to finish.", info.GoalScore - score);
				case 2: // equal
					if (score > info.GoalScore)
						return string.Format("You needed exactly {0:0} fewer points to finish.", info.GoalScore);
					else
						return string.Format("You needed exactly {0:0} more points to finish.", info.GoalScore);
				case 3: // less
					return string.Format("You needed {0:0} fewer points to finish.", score - info.GoalScore);
			}
			return "Unknown goal";
		}

		public static bool Test(string level, int score)
		{
			var info = MyFiles.LoadLevelInfo(level);
			switch (info.GoalType)
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
