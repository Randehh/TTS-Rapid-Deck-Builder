using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Renderers {
	public interface ICardRenderer {
		void RenderDeck(DeckViewModel deck, string filePath = "");
		void RenderCard(DeckViewModel deck, IDeckCardViewModel card, string filePath = "");
		Action<BitmapSource> OnDeckRendered { get; set; }
		Action<BitmapSource> OnCardRendered { get; set; }
	}
}
