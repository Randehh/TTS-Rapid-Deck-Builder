using System.Windows;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Views {
	/// <summary>
	/// Interaction logic for TextDialog.xaml
	/// </summary>
	public partial class DeckCreationDialog : Window {
		public DeckCreationDialog() {
			InitializeComponent();
		}

		public static DeckConfig ShowCreationDialog() {
			DeckCreationDialog dialog = new DeckCreationDialog();
			DeckCreationDialogViewModel vm = new DeckCreationDialogViewModel((success) => {
				dialog.DialogResult = success;
				dialog.Close();
			});
			dialog.DataContext = vm;
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value) {
				return vm.GetConfig();
			}
			return null;
		}
	}
}
