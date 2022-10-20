using System;
using System.Windows.Controls;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Views
{
    /// <summary>
    /// Interaction logic for DeckGridView.xaml
    /// </summary>
    public partial class DeckGridView : UserControl {
        public DeckGridView() {
            InitializeComponent();
        }

        private void DeckGrid_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
            if(DataContext is DeckViewModel deckVM) {
                deckVM.DeckGridControl = DeckGrid;
                deckVM.GenerateGridColumns();
            }
        }
    }
}
