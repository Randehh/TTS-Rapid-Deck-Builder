using RondoFramework.BaseClasses;
using System.Collections.Generic;
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
				m_Deck.CreateBitmaps();
			}
		}

		private DeckViewModel m_Deck;

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

		public DeckSettingsViewModel(DeckViewModel deck) {
			m_Deck = deck;
			m_Deck.DeckConfig = deck.DeckConfig;

			foreach(DeckConfig c in DeckConfigs) {
				if(m_Deck.DeckConfig.Width == c.Width && m_Deck.DeckConfig.Height == c.Height) {
					SelectedDeckConfig = c;
					break;
				}
			}

			BrowseBackgroundCommand = new SimpleCommand(BrowseBackground);
		}

		private void BrowseBackground(object o) {
			string pngFile = BrowserUtilities.BrowseForFile($"PNG files (*.png)|*.png");
			CustomBackground = pngFile;
		}

		private bool IsBackgroundValid(string imagePath) {
			if (!File.Exists(imagePath)) return false;
			return true;
		}
	}
}
