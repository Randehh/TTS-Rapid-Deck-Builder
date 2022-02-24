using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Renderers {
	public interface ICardRenderer {
		Task RenderDeck(DeckViewModel deck);
		Task RenderCard(DeckViewModel deck, IDeckCardViewModel card);
		Action<BitmapSource> OnDeckRendered { get; set; }
		Action<BitmapSource> OnCardRendered { get; set; }
	}
}
