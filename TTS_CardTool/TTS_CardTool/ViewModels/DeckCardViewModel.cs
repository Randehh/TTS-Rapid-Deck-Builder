using RondoFramework.BaseClasses;
using System.Windows.Input;

namespace TTS_CardTool.ViewModels {
	public class DeckCardViewModel : ViewModelBase, IDeckCardViewModel {

		public DeckCardViewModel() {
			CountIncreaseCommand = new SimpleCommand((o) => Count++);
			CountDecreaseCommand = new SimpleCommand((o) => Count--, () => Count > 1);
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

		private string m_Title = null;
		public string Title {
			get => m_Title;
			set => SetProperty(ref m_Title, value);
		}

		private string m_Description = null;
		public string Description {
			get => m_Description;
			set => SetProperty(ref m_Description, value);
		}

		private int m_Count = 1;
		public int Count {
			get => m_Count;
			set => SetProperty(ref m_Count, value);
		}

		private bool m_IsChild = false;
		public bool IsChild {
			get => m_IsChild;
			set => SetProperty(ref m_IsChild, value);
		}
	}
}
