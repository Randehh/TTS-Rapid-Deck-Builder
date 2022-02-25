using RondoFramework.BaseClasses;
using System.Diagnostics;
using System.Windows.Input;

namespace TTS_CardTool.ViewModels {
	public class AboutDialogViewModel : ViewModelBase {

		private const string GITHUB_URL = @"https://github.com/Randehh";
		private const string KOFI_URL = @"https://ko-fi.com/rondo_dev";

		private ICommand m_DonateCommand;
		public ICommand DonateCommand {
			get => m_DonateCommand;
			set => SetProperty(ref m_DonateCommand, value);
		}

		private ICommand m_GitHubCommand;
		public ICommand GitHubCommand {
			get => m_GitHubCommand;
			set => SetProperty(ref m_GitHubCommand, value);
		}

		public AboutDialogViewModel() {
			GitHubCommand = new SimpleCommand((o) => OpenURL(GITHUB_URL));
			DonateCommand = new SimpleCommand((o) => OpenURL(KOFI_URL));
		}

		private void OpenURL(string link) {
			Process.Start(link);
		}
	}
}
