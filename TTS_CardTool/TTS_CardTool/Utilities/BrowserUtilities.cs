using System.Windows.Forms;

namespace TTS_CardTool.Utilities {
	public static class BrowserUtilities {
		public static string BrowseForFile(string filter = "All files (*.*)|*.*") {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = filter;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    return openFileDialog.FileName;
                }
            }
            return "";
        }
	}
}
