using System.Windows;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			DataContext = new MainWindowViewModel();
		}
	}
}
