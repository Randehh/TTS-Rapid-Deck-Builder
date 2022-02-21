using System.Drawing;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.Utilities {
	public static class DrawingUtilities {

		private const int CARD_MARGIN = 16;
		private static StringFormat DefaultStringFormat => new StringFormat() { Alignment = StringAlignment.Center };
		private static Font StartFont => new Font("Arial", 60);
		private static SolidBrush CardBackgroundBrush => new SolidBrush(Color.Snow);

		public static void DrawCard(Bitmap bitmap, IDeckCardViewModel card, float cardWidth, float cardHeight, float offsetX = 0, float offsetY = 0) {
			using (Graphics gfx = Graphics.FromImage(bitmap)) {
				DrawCard(gfx, card, cardWidth, cardHeight, offsetX, offsetY);
			}
		}

		public static void DrawCard(Graphics gfx, IDeckCardViewModel card, float cardWidth, float cardHeight, float offsetX = 0, float offsetY = 0) {
			RectangleF cardRect = new RectangleF(
				offsetX,
				offsetY,
				cardWidth,
				cardHeight);

			Rectangle fullCardRect = new Rectangle((int)cardRect.X, (int)cardRect.Y, (int)cardRect.Width, (int)cardRect.Height);
			gfx.FillRectangle(CardBackgroundBrush, fullCardRect);
			gfx.DrawRectangle(new Pen(Color.Black, 4), fullCardRect);

			if (card != null) {
				RectangleF cardRectMargins = AddMarginToRect(cardRect, CARD_MARGIN);

				RectangleF titleRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y, cardRectMargins.Width, cardRectMargins.Height * 0.25f);
				RectangleF descriptionRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y + cardRectMargins.Height * 0.25f, cardRectMargins.Width, cardRectMargins.Height * 0.75f);

				WriteStringInRegion(gfx, card.Title, titleRegion, StartFont, DefaultStringFormat);
				WriteStringInRegion(gfx, card.Description, descriptionRegion, StartFont, DefaultStringFormat);
			}
		}

		public static void WriteStringInRegion(Graphics gfx, string text, RectangleF region, Font font, StringFormat stringFormat = null) {
			if (string.IsNullOrWhiteSpace(text)) return;

			if (stringFormat == null) {
				stringFormat = DefaultStringFormat;
			}

			text = text.Replace(@"\n", "\n");
			font = GetFontForText(gfx, text, font, region.Width);
			SizeF titleSize = gfx.MeasureString(text, font, int.MaxValue);
			RectangleF titleRect = GetCenteredRectangle(titleSize.Width, titleSize.Height, region, centerHorizontal:false);
			gfx.DrawString(text, font, Brushes.Black, titleRect, stringFormat);
		}

		public static Font GetFontForText(Graphics gfx, string text, Font font, float width) {
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

		public static RectangleF GetCenteredRectangle(float elementWidth, float elementHeight, RectangleF rectRegion, bool centerHorizontal = true, bool centerVertical = true) {
			float offsetX = (rectRegion.Width - elementWidth) * (centerHorizontal ? 0.5f : 0f);
			float offsetY = (rectRegion.Height - elementHeight) * (centerVertical ? 0.5f : 0f);
			return new RectangleF(rectRegion.X + offsetX, rectRegion.Y + offsetY, rectRegion.Width, rectRegion.Height);
		}

		public static RectangleF AddMarginToRect(RectangleF rect, float margin) {
			return new RectangleF(
							rect.X + margin,
							rect.Y + margin,
							rect.Width - (margin * 2),
							rect.Height - (margin * 2));
		}
	}
}