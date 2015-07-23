using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace TheCubers
{
	public class UIOptions : UIMenu
	{
		[System.Serializable]
		private struct Settings
		{
			public bool Fullscreen;
			public bool VSync;
			public int Width;
			public int Height;
			public int Quality;
			// note LitJson can not do floats...
			public double AudioEffects;
			public double AudioMusic;

			public static Settings Default()
			{
				return new Settings()
				{
					Fullscreen = true,
					VSync = true,
					Width = Screen.resolutions[Screen.resolutions.Length - 1].width,
					Height = Screen.resolutions[Screen.resolutions.Length - 1].height,
					Quality = 4,
					AudioEffects = 1f,
					AudioMusic = 1f,
				};
			}
			public static bool operator ==(Settings l, Settings r)
			{
				return l.Fullscreen == r.Fullscreen
					&& l.VSync == r.VSync
					&& l.Width == r.Width
					&& l.Height == r.Height
					&& l.Quality == r.Quality
					&& System.Math.Round(1000d * l.AudioEffects) == System.Math.Round(1000d * r.AudioEffects)
					&& System.Math.Round(1000d * l.AudioMusic) == System.Math.Round(1000d * r.AudioMusic);
			}
			public static bool operator !=(Settings l, Settings r)
			{ return !(l == r); }
			public override bool Equals(object obj)
			{ return base.Equals(obj); }
			public override int GetHashCode()
			{ return base.GetHashCode(); }

		}

		private Settings current;
		private Settings temp;

		// ui items
		public Text BackText;
		public Toggle FullscreenUI;
		public Toggle VSyncUI;
		public Slider ResolutionSlider;
		public Text ResolutionText;
		public Slider QualitySlider;
		public Text QualityValue;
		public Slider AudioEffectsSlider;
		public Text AudioEffectsValue;
		public Slider AudioMusicSlider;
		public Text AudioMusicValue;

		private static string settingsFile { get { return Application.persistentDataPath + "/settings.json"; } }

		void Start()
		{
			ResolutionSlider.minValue = -1;
			ResolutionSlider.maxValue = Screen.resolutions.Length - 1;
			setupOnChange();
			UIOptions.Load(this);
		}

		protected override void OnOpenStart()
		{
			temp = current;
			applyUI(current);
			Changed();
		}

		/// <summary>Load and apply settings from the settings file.</summary>
		/// <param name="obj">Load in to this object</param>
		public static void Load(UIOptions obj)
		{
			Settings settings;
			if (File.Exists(settingsFile))
			{
				Debug.Log("Loading settings from: " + settingsFile);
				string jsonData;
				using (var r = new StreamReader(settingsFile))
				{
					jsonData = r.ReadToEnd();
				}
				settings = LitJson.JsonMapper.ToObject<Settings>(jsonData);
			}
			else
			{
				Debug.Log("Using Default, settings file does not exist: " + settingsFile);
				settings = Settings.Default();
			}
			if (obj)
				obj.apply(settings);
			else
				UIOptions.applyOnly(settings);
		}

		private void apply(Settings settings)
		{
			current = settings;
			temp = current;

			// update UI
			applyUI(current);

			// apply the real settings
			UIOptions.applyOnly(current);

			Changed();
		}
		private void applyUI(Settings settings)
		{
			FullscreenUI.isOn = current.Fullscreen;
			FullscreenUI.onValueChanged.Invoke(FullscreenUI.isOn);

			VSyncUI.isOn = current.VSync;
			VSyncUI.onValueChanged.Invoke(VSyncUI.isOn);

			ResolutionSlider.value = findResolution(current.Width, current.Height);
			ResolutionSlider.onValueChanged.Invoke(ResolutionSlider.value);

			QualitySlider.value = current.Quality;
			QualitySlider.onValueChanged.Invoke(QualitySlider.value);

			AudioEffectsSlider.value = (float)current.AudioEffects;
			AudioEffectsSlider.onValueChanged.Invoke(AudioEffectsSlider.value);
			AudioMusicSlider.value = (float)current.AudioMusic;
			AudioMusicSlider.onValueChanged.Invoke(AudioMusicSlider.value);

		}
		private static void applyOnly(Settings settings)
		{
			//Screen.fullScreen = settings.Fullscreen;
			QualitySettings.vSyncCount = settings.VSync ? 1 : 0;
			QualitySettings.SetQualityLevel(settings.Quality);
			Screen.SetResolution(settings.Width, settings.Height, settings.Fullscreen);
			// ToDo  2: Audio volume
		}
		public void Apply()
		{
			apply(temp);

			Debug.Log("Saving settings to: " + settingsFile);
			string jsonData = LitJson.JsonMapper.ToJson(current);
			using (var w = new StreamWriter(settingsFile, false))
			{
				w.Write(jsonData);
			}
		}

		private int findResolution(int width, int height)
		{
			for (int i = 0; i < Screen.resolutions.Length; ++i)
			{
				if (width == Screen.resolutions[i].width && height == Screen.resolutions[i].height)
					return i;
			}
			return -1;
		}

		private void setupOnChange()
		{
			FullscreenUI.onValueChanged.AddListener(delegate(bool v)
			{
				temp.Fullscreen = v;
				Changed();
			});

			VSyncUI.onValueChanged.AddListener(delegate(bool v)
			{
				temp.VSync = v;
				Changed();
			});

			ResolutionSlider.onValueChanged.AddListener(delegate(float v)
			{
				int val = Mathf.RoundToInt(v);
				if (val != -1)
				{
					temp.Width = Screen.resolutions[val].width;
					temp.Height = Screen.resolutions[val].height;
				}
				ResolutionText.text = temp.Width + "x" + temp.Height;
				Changed();
			});

			QualitySlider.onValueChanged.AddListener(delegate(float v)
			{
				int val = Mathf.RoundToInt(v);
				QualityValue.text = QualitySettings.names[val];
				temp.Quality = val;
				Changed();
			});

			AudioEffectsSlider.onValueChanged.AddListener(delegate(float value)
			{
				value = Mathf.Clamp01(value);
				AudioEffectsValue.text = Mathf.Round(value * 100f) + "%";
				temp.AudioEffects = value;
				Changed();
			});

			AudioMusicSlider.onValueChanged.AddListener(delegate(float value)
			{
				value = Mathf.Clamp01(value);
				AudioMusicValue.text = Mathf.Round(value * 100f) + "%";
				temp.AudioMusic = value;
				Changed();
			});
		}

		public void Changed()
		{
			if (temp == current)
				BackText.text = "Back";
			else
				BackText.text = "Cancel";
		}
	}
}