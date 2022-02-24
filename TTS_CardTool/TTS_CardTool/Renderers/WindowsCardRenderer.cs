using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Renderers {
	public class WindowsCardRenderer : ICardRenderer {
		private const int CARD_MARGIN = 16;
		private StringFormat DefaultStringFormat => new StringFormat() { Alignment = StringAlignment.Center };
		private Font GetFont(string font) => new Font(font, 60);
		private SolidBrush CardBackgroundBrush => new SolidBrush(Color.Snow);

		public Action<BitmapSource> OnDeckRendered { get; set; } = delegate { };
		public Action<BitmapSource> OnCardRendered { get; set; } = delegate { };

		public Task RenderDeck(DeckViewModel deck) {
			Bitmap bitmap = new Bitmap(deck.ImageWidth, deck.ImageHeight);
			using (Graphics gfx = Graphics.FromImage(bitmap)) {
				for (int row = 0; row < DeckViewModel.CARD_COUNT_VERTICAL; row++) {
					for (int column = 0; column < DeckViewModel.CARD_COUNT_HORIZONTAL; column++) {
						int cardIndex = column + (row * DeckViewModel.CARD_COUNT_HORIZONTAL);
						DrawCard(
							gfx,
							deck,
							deck.CardDisplayList.Count > cardIndex ? deck.CardDisplayList[cardIndex] : null,
							deck.CardWidth * column,
							deck.CardHeight * row);
					}
				}

				gfx.Flush();
			}

			Application.Current.Dispatcher.Invoke(() => {
				OnDeckRendered(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
			});

			bitmap.Save(DeckViewModel.LOCAL_RENDER_FILE_NAME, ImageFormat.Png);
			return Task.CompletedTask;
		}

		public Task RenderCard(DeckViewModel deck, IDeckCardViewModel card) {
			Bitmap bitmap = new Bitmap((int)deck.CardWidth, (int)deck.CardHeight);
			using (Graphics gfx = Graphics.FromImage(bitmap)) {
				DrawCard(gfx, deck, card);
				gfx.Flush();
			}

			Application.Current.Dispatcher.Invoke(() => {
				OnCardRendered(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
			});
			return Task.CompletedTask;
		}

		public void DrawCard(
			Graphics gfx,
			DeckViewModel deck,
			IDeckCardViewModel card,
			float offsetX = 0,
			float offsetY = 0) {

			float cardWidth = deck.CardWidth;
			float cardHeight = deck.CardHeight;
			string customBackground = deck.DeckConfig.CustomBackground;
			string fontType = deck.DeckConfig.Font;

			gfx.InterpolationMode = InterpolationMode.High;
			gfx.SmoothingMode = SmoothingMode.HighQuality;
			gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			gfx.CompositingQuality = CompositingQuality.HighQuality;

			RectangleF cardRect = new RectangleF(
				offsetX,
				offsetY,
				cardWidth,
				cardHeight);

			Rectangle fullCardRect = new Rectangle((int)cardRect.X, (int)cardRect.Y, (int)cardRect.Width, (int)cardRect.Height);
			if (!string.IsNullOrWhiteSpace(customBackground)) {
				gfx.DrawImage(Image.FromFile(customBackground), fullCardRect);
			} else {
				gfx.FillRectangle(CardBackgroundBrush, fullCardRect);
				gfx.DrawRectangle(new Pen(Color.Black, 4), fullCardRect);
			}

			if (card != null) {
				RectangleF cardRectMargins = AddMarginToRect(cardRect, CARD_MARGIN);

				RectangleF titleRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y, cardRectMargins.Width, cardRectMargins.Height * 0.25f);
				RectangleF descriptionRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y + cardRectMargins.Height * 0.25f, cardRectMargins.Width, cardRectMargins.Height * 0.75f);

				Font font = GetFont(fontType);
				WriteStringInRegion(gfx, card.Title, titleRegion, font, deck.DeckConfig.OutlineSize, DefaultStringFormat);
				WriteStringInRegion(gfx, card.Description, descriptionRegion, font, deck.DeckConfig.OutlineSize, DefaultStringFormat);
			}
		}

		public void WriteStringInRegion(Graphics gfx, string text, RectangleF region, Font font, int outlineSize, StringFormat stringFormat = null) {
			if (string.IsNullOrWhiteSpace(text)) return;

			if (stringFormat == null) {
				stringFormat = DefaultStringFormat;
			}

			text = text.Replace(@"\n", "\n");
			font = GetFontForText(gfx, text, font, region.Width);
			SizeF textSize = gfx.MeasureString(text, font, int.MaxValue);
			RectangleF textRect = GetCenteredRectangle(textSize.Width, textSize.Height, region, centerHorizontal: false);

			GraphicsPath path = new GraphicsPath();
			path.AddString(text, font.FontFamily, (int)System.Drawing.FontStyle.Regular, font.Size, textRect, stringFormat);
			gfx.DrawPath(new Pen(Brushes.Black, outlineSize), path);
			gfx.FillPath(Brushes.White, path);
		}

		public Font GetFontForText(Graphics gfx, string text, Font font, float width) {
			if (string.IsNullOrWhiteSpace(text)) return font;

			float initialSize = gfx.MeasureString(text, font).Width;
			bool isSizeDownFunction = initialSize > width;
			float sizeMod = isSizeDownFunction ? -0.2f : 0.2f;

			while (isSizeDownFunction ? gfx.MeasureString(text, font).Width > width : gfx.MeasureString(text, font).Width < width) {
				if (font.Size >= 80) return font;
				font = new Font(font.FontFamily.Name, font.Size + sizeMod);
			}
			return font;
		}

		public RectangleF GetCenteredRectangle(float elementWidth, float elementHeight, RectangleF rectRegion, bool centerHorizontal = true, bool centerVertical = true) {
			float offsetX = (rectRegion.Width - elementWidth) * (centerHorizontal ? 0.5f : 0f);
			float offsetY = (rectRegion.Height - elementHeight) * (centerVertical ? 0.5f : 0f);
			return new RectangleF(rectRegion.X + offsetX, rectRegion.Y + offsetY, rectRegion.Width, rectRegion.Height);
		}

		public RectangleF AddMarginToRect(RectangleF rect, float margin) {
			return new RectangleF(
							rect.X + margin,
							rect.Y + margin,
							rect.Width - (margin * 2),
							rect.Height - (margin * 2));
		}
	}
}
