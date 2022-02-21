using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System.Collections.ObjectModel;
using TTS_CardTool.Views;

namespace TTS_CardTool.ViewModels {
	public class TTSProjectViewModel : ViewModelBase, IProjectModule {
		private string m_ProjectName = null;
		public string ProjectName {
			get => m_ProjectName;
			set => SetProperty(ref m_ProjectName, value);
		}

		private DeckViewModel m_SelectedDeck = null;
		public DeckViewModel SelectedDeck {
			get => m_SelectedDeck;
			set => SetProperty(ref m_SelectedDeck, value);
		}

		private ObservableCollection<DeckViewModel> m_DeckList = new ObservableCollection<DeckViewModel>();
		public ObservableCollection<DeckViewModel> DeckList {
			get => m_DeckList;
			set => SetProperty(ref m_DeckList, value);
		}

		private SimpleCommand m_CreateNewDeck;
		public SimpleCommand CreateNewDeck {
			get => m_CreateNewDeck;
			set => SetProperty(ref m_CreateNewDeck, value);
		}

		public TTSProjectViewModel() {
			CreateNewDeck = new SimpleCommand((o) => AddNewDeck());
		}

		private void AddNewDeck() {
			string deckName = TextDialog.ShowDialog("Create new deck", "Type the name of your new deck");
			if (string.IsNullOrWhiteSpace(deckName)) return;

			DeckList.Add(new DeckViewModel() { DeckName = deckName });
		}

		public string ModuleName => "TTS_Project";
	}
}