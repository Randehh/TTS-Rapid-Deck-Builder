using RondoFramework.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TTS_CardTool.ViewModels {
	public class TextDialogViewModel : ViewModelBase {
		private string m_DialogTitle;
		public string DialogTitle {
			get => m_DialogTitle;
			set => SetProperty(ref m_DialogTitle, value);
		}

		private string m_DescriptionText;
		public string DescriptionText {
			get => m_DescriptionText;
			set => SetProperty(ref m_DescriptionText, value);
		}

		private string m_InputText;
		public string InputText {
			get => m_InputText;
			set => SetProperty(ref m_InputText, value);
		}

		private ICommand m_ConfirmCommand;
		public ICommand ConfirmCommand {
			get => m_ConfirmCommand;
			set => SetProperty(ref m_ConfirmCommand, value);
		}

		private Action m_RequestCloseAction;

		public TextDialogViewModel(string dialogTitle, string descriptionText, Action requestCloseAction) {
			DialogTitle = dialogTitle;
			DescriptionText = descriptionText;
			m_RequestCloseAction = requestCloseAction;

			ConfirmCommand = new SimpleCommand(Confirm, () => !string.IsNullOrWhiteSpace(InputText));
		}

		public void Confirm(object o) {
			InputText = InputText.Trim();
			m_RequestCloseAction();
		}
	}
}
