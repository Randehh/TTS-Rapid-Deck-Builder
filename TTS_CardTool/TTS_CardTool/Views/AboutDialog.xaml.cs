using System.Windows;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Views {
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window {
		public AboutDialog() {
			InitializeComponent();
			DataContext = new AboutDialogViewModel();
		}
	}
}
