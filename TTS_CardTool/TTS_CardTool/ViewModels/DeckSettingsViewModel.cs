using RondoFramework.BaseClasses;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using TTS_CardTool.Utilities;

namespace TTS_CardTool.ViewModels {
	public class DeckSettingsViewModel : ViewModelBase {
		public List<DeckConfig> DeckConfigs => DeckConfig.GetStandardDeckConfigs();

		private DeckConfig m_SelectedDeckConfig;
		public DeckConfig SelectedDeckConfig {
			get => m_SelectedDeckConfig;
			set {
				SetProperty(ref m_SelectedDeckConfig, value);
				m_Deck.DeckConfig.Width = SelectedDeckConfig.Width;
				m_Deck.DeckConfig.Height = SelectedDeckConfig.Height;
				m_Deck.InvokeRenderers();
			}
		}

		private DeckViewModel m_Deck;
		public DeckViewModel Deck {
			get => m_Deck;
			set => m_Deck = value;
		}

		public string Name {
			get => m_Deck.DeckConfig.DisplayName;
			set => m_Deck.DeckConfig.DisplayName = value;
		}

		private ICommand m_BrowseBackgroundCommand;
		public ICommand BrowseBackgroundCommand {
			get => m_BrowseBackgroundCommand;
			set => SetProperty(ref m_BrowseBackgroundCommand, value);
		}

		public string CustomBackground {
			get => m_Deck.DeckConfig.CustomBackground;
			set {
				if (!IsBackgroundValid(value)) return;
				m_Deck.DeckConfig.CustomBackground = value;
				OnPropertyChanged(nameof(CustomBackground));
			}
		}

		private List<string> m_FontList = new List<string>();
		public List<string> FontList {
			get => m_FontList;
			set => SetProperty(ref m_FontList, value);
		}

		public string Font {
			get => m_Deck.DeckConfig.Font;
			set => m_Deck.DeckConfig.Font = value;
		}

		public string OutlineSize {
			get => m_Deck.DeckConfig.OutlineSize.ToString();
			set {
				int result;
				if(int.TryParse(value, out result)){
					m_Deck.DeckConfig.OutlineSize = result;
				}
			}
		}

		public DeckSettingsViewModel(DeckViewModel deck) {
			m_Deck = deck;
			m_Deck.DeckConfig = deck.DeckConfig;

			foreach(DeckConfig c in DeckConfigs) {
				if(m_Deck.DeckConfig.Width == c.Width && m_Deck.DeckConfig.Height == c.Height) {
					SelectedDeckConfig = c;
					break;
				}
			}

			List<string> fonts = new List<string>();
			foreach (FontFamily font in FontFamily.Families) {
				fonts.Add(font.Name);
			}
			FontList = fonts;

			BrowseBackgroundCommand = new SimpleCommand(BrowseBackground);
		}

		private void BrowseBackground(object o) {
			string pngFile = BrowserUtilities.BrowseForFile($"PNG files (*.png)|*.png");
			CustomBackground = pngFile;
		}

		private bool IsBackgroundValid(string imagePath) {
			if (string.IsNullOrWhiteSpace(imagePath)) return true;
			if (!File.Exists(imagePath)) return false;
			return true;
		}
	}
}
