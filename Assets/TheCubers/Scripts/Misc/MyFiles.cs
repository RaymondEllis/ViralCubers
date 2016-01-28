using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

// Structs and stuff related to loading/saving json files.

namespace TheCubers
{
	public struct LevelInfo
	{
		public string Level, Title, Description;
		public int GoalType;
		public int GoalScore;
	}

	public struct LevelScores
	{
		public string Name;
		public List<string> Names;
		public List<int> Scores;
	}

	public class Profile
	{
		public string Name;
		public DateTime LastUsed;
		public int Completed;
	}

	// note: settings.json is still handled in UIOptions.
	public static class MyFiles
	{
		private static string profileFile { get { return Application.persistentDataPath + "/profiles.json"; } }
		private static string scoresFile { get { return Application.persistentDataPath + "/leaderboard.json"; } }

		private static LevelInfo[] levelInfo = null;


		// Profiles
		public static List<Profile> LoadProfiles()
		{
			if (!File.Exists(profileFile))
				return null;

			Debug.Log("Loading profiles from: " + profileFile);
			string jsonData;
			using (var r = new StreamReader(profileFile))
			{
				jsonData = r.ReadToEnd();
			}
			return new List<Profile>(LitJson.JsonMapper.ToObject<Profile[]>(jsonData));
		}
		public static void SaveProfiles(List<Profile> profiles)
		{
			string jsonData;
			jsonData = LitJson.JsonMapper.ToJson(profiles);
			using (var w = new StreamWriter(profileFile, false))
			{
				w.Write(jsonData);
			}
		}

		// Level Info
		public static LevelInfo LoadLevelInfo(string level)
		{
#if UNITY_EDITOR || DEBUG
			levelInfo = null;
#endif
			if (levelInfo == null)
				LoadLevelInfos();
			for (int i = 0; i < levelInfo.Length; ++i)
				if (levelInfo[i].Level == level)
					return levelInfo[i];
			return new LevelInfo();
		}
		public static LevelInfo[] LoadLevelInfos()
		{
#if UNITY_EDITOR || DEBUG
			levelInfo = null;
#endif
			if (levelInfo == null)
			{
				Debug.Log("Loading level info");
				var infoFile = Resources.Load<TextAsset>("level info");
				levelInfo = LitJson.JsonMapper.ToObject<LevelInfo[]>(infoFile.text);
			}
			return levelInfo;
		}

		// Level Scores
		public static List<LevelScores> LoadLevelScores()
		{
			if (!File.Exists(scoresFile))
				return null;

			Debug.Log("Loading leaderboards: " + scoresFile);
			string data = File.ReadAllText(scoresFile);
			return LitJson.JsonMapper.ToObject<List<LevelScores>>(data);
		}
		public static void SaveLevelScores(List<LevelScores> levels)
		{
			Debug.Log("Writing leaderboards: " + scoresFile);
			string data = LitJson.JsonMapper.ToJson(levels);
			File.WriteAllText(scoresFile, data);
		}
	}
}