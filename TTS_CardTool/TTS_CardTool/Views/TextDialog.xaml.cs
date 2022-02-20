using System.Windows;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Views {
	/// <summary>
	/// Interaction logic for TextDialog.xaml
	/// </summary>
	public partial class TextDialog : Window {
		public TextDialog() {
			InitializeComponent();
		}

		public static string ShowDialog(string title, string description) {
			TextDialog dialog = new TextDialog();
			TextDialogViewModel vm = new TextDialogViewModel(title, description, () => {
				dialog.DialogResult = true;
				dialog.Close();
			});
			dialog.DataContext = vm;
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value) {
				return vm.InputText;
			}
			return "";
		}
	}
}
