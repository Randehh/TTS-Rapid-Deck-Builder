using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
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

		private List<MenuItem> m_DeckContextMenuItems;
		public List<MenuItem> DeckContextMenuItems {
			get => m_DeckContextMenuItems;
			set => SetProperty(ref m_DeckContextMenuItems, value);
		}

		private ICommand m_CreateNewDeck;
		public ICommand CreateNewDeck {
			get => m_CreateNewDeck;
			set => SetProperty(ref m_CreateNewDeck, value);
		}

		private ICommand m_RemoveDeck;
		public ICommand RemoveDeckCommand {
			get => m_RemoveDeck;
			set => SetProperty(ref m_RemoveDeck, value);
		}

		private ICommand m_RenameDeckCommand;
		public ICommand RenameDeckCommand {
			get => m_RenameDeckCommand;
			set => SetProperty(ref m_RenameDeckCommand, value);
		}

		public TTSProjectViewModel() {
			CreateNewDeck = new SimpleCommand((o) => AddNewDeck());
			RemoveDeckCommand = new SimpleCommand(RemoveDeck);
			RenameDeckCommand = new SimpleCommand(RenameDeck);

			DeckContextMenuItems = new List<MenuItem>() {
				new MenuItem() { Header = "Rename", Command = RenameDeckCommand }
			};
		}

		private void AddNewDeck() {
			DeckConfig config = DeckCreationDialog.ShowCreationDialog();
			if (config == null) return;

			DeckList.Add(new DeckViewModel(config));
		}

		private void RemoveDeck(object o) {
			DeckViewModel deck = o as DeckViewModel;
			DeckList.Remove(deck);
		}

		private void RenameDeck(object o) {
			string newName = TextDialog.ShowDialog("Rename deck", "Type a new name for the deck");
			if (string.IsNullOrWhiteSpace(newName)) return;
			SelectedDeck.DeckConfig.DisplayName = newName;
		}

		public string ModuleName => "TTS_Project";
	}
}