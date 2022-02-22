using RondoFramework.BaseClasses;
using System.Collections.Generic;

namespace TTS_CardTool.ViewModels {
	public class DeckConfig : ViewModelBase {

		private static List<DeckConfig> m_CachedList;
		public static List<DeckConfig> GetStandardDeckConfigs() {
			if(m_CachedList == null) {
				m_CachedList = new List<DeckConfig>() {
					new DeckConfig() { DisplayName = "Rectangle (4096 x 4096)", Height = 4096, Width = 4096 },
					new DeckConfig() { DisplayName = "Rectangle (2048 x 2048)", Height = 2048, Width = 2048 },
					new DeckConfig() { DisplayName = "Square (2884 x 4096)", Height = 2884, Width = 4096 },
					new DeckConfig() { DisplayName = "Square (1442 x 2048)", Height = 1442, Width = 2048 },
				};
			}
			return m_CachedList;
		}

		private string m_DisplayName;
		public string DisplayName {
			get => m_DisplayName;
			set => SetProperty(ref m_DisplayName, value);

		}
		private int m_Width;
		public int Width {
			get => m_Width;
			set => SetProperty(ref m_Width, value);
		}

		private int m_Height;
		public int Height {
			get => m_Height;
			set => SetProperty(ref m_Height, value);
		}

		public DeckConfig Copy() {
			return new DeckConfig() {
				DisplayName = DisplayName,
				Width = Width,
				Height = Height,
			};
		}
	}
}
