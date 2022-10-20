using RondoFramework.BaseClasses;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace TTS_CardTool.ViewModels {
	public class DeckCardViewModel : ViewModelBase, IDeckCardViewModel {

		public DeckCardViewModel() {
			CountIncreaseCommand = new SimpleCommand((o) => Count++);
			CountDecreaseCommand = new SimpleCommand((o) => Count--, () => Count > 1);

			PropertyChanged += DeckCardViewModel_PropertyChanged;
		}

		private void DeckCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			OnCardUpdated();
		}

		private ICommand m_CountIncreaseCommand;
		public ICommand CountIncreaseCommand {
			get => m_CountIncreaseCommand;
			set => SetProperty(ref m_CountIncreaseCommand, value);
		}

		private ICommand m_CountDecreaseCommand;
		public ICommand CountDecreaseCommand {
			get => m_CountDecreaseCommand;
			set => SetProperty(ref m_CountDecreaseCommand, value);
		}

		private Dictionary<string, string> m_CardValues = new Dictionary<string, string>();
		public Dictionary<string, string> CardValues
        {
			get => m_CardValues;
			set => SetProperty(ref m_CardValues, value);
        }

		private int m_Count = 1;
		public int Count {
			get => m_Count;
			set {
				OnCountUpdated(this, m_Count, value);
				SetProperty(ref m_Count, value);
			}
		}

		private bool m_IsChild = false;
		public bool IsChild {
			get => m_IsChild;
			set => SetProperty(ref m_IsChild, value);
		}

		public Action OnCardUpdated { get; set; } = delegate { };
		public Action<DeckCardViewModel, int, int> OnCountUpdated { get; set; } = delegate { };
	}
}