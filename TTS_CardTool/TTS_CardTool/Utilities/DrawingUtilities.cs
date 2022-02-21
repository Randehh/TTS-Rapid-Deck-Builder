using System.Drawing;

namespace TTS_CardTool.Utilities {
	public static class DrawingUtilities {

		private static StringFormat DefaultStringFormat => new StringFormat() { Alignment = StringAlignment.Center };

		public static void WriteStringInRegion(Graphics gfx, string text, RectangleF region, Font font, StringFormat stringFormat = null) {
			if (string.IsNullOrWhiteSpace(text)) return;

			if (stringFormat == null) {
				stringFormat = DefaultStringFormat;
			}

			text = text.Replace(@"\n", "\n");
			font = GetFontForText(gfx, text, font, region.Width);
			SizeF titleSize = gfx.MeasureString(text, font, int.MaxValue);
			RectangleF titleRect = GetCenteredRectangle(titleSize.Width, titleSize.Height, region);
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

		public static RectangleF GetCenteredRectangle(float elementWidth, float elementHeight, RectangleF rectRegion) {
			float offsetX = (rectRegion.Width - elementWidth) * 0.5f;
			float offsetY = (rectRegion.Height - elementHeight) * 0.5f;
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