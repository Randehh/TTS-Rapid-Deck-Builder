using RondoFramework.BaseClasses;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace TTS_CardTool.ViewModels {
	public class DeckCreationDialogViewModel : ViewModelBase {

		private string m_DeckName;
		public string DeckName {
			get => m_DeckName;
			set => SetProperty(ref m_DeckName, value);
		}

		public List<DeckConfig> DeckConfigs => DeckConfig.GetStandardDeckConfigs();

		private DeckConfig m_SelectedDeckConfig;
		public DeckConfig SelectedDeckConfig {
			get => m_SelectedDeckConfig;
			set => SetProperty(ref m_SelectedDeckConfig, value);
		}

		private ICommand m_ConfirmCommand;
		public ICommand ConfirmCommand {
			get => m_ConfirmCommand;
			set => SetProperty(ref m_ConfirmCommand, value);
		}

		private ICommand m_CancelCommand;
		public ICommand CancelCommand {
			get => m_CancelCommand;
			set => SetProperty(ref m_CancelCommand, value);
		}

		private Action<bool> m_OnRequestClose;

		public DeckConfig GetConfig() {
			DeckConfig config = SelectedDeckConfig.Copy();
			config.DisplayName = DeckName;
			return config;
		}

		public DeckCreationDialogViewModel(Action<bool> onRequestClose) {
			m_OnRequestClose = onRequestClose;

			ConfirmCommand = new SimpleCommand(OnConfirmCommand);
			CancelCommand = new SimpleCommand(OnCancelCommand);

			SelectedDeckConfig = DeckConfigs[0];
		}

		private void OnConfirmCommand(object o) {
			m_OnRequestClose(true);
		}

		private void OnCancelCommand(object o) {
			m_OnRequestClose(false);
		}
	}
}
