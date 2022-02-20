using RondoFramework.BaseClasses;

namespace TTS_CardTool.ViewModels {
	public class DeckCardViewModel : ViewModelBase {
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
	}
}
