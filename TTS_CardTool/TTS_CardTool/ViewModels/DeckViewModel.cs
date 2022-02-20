using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using TTS_CardTool.Utilities;

namespace TTS_CardTool.ViewModels {
	public class DeckViewModel : ViewModelBase, IProjectModule {
		public string ModuleName => $"Deck_{DeckName}";

		private const int CARD_COUNT_HORIZONTAL = 10;
		private const int CARD_COUNT_VERTICAL = 7;
		private const int IMAGE_WIDTH = 4096;
		private const int IMAGE_HEIGHT = 4096;
		private const int CARD_WIDTH = IMAGE_WIDTH / CARD_COUNT_HORIZONTAL;
		private const int CARD_HEIGHT = IMAGE_HEIGHT / CARD_COUNT_VERTICAL;

		private Bitmap m_ImageBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
		private Font m_StartFont = new Font("Arial", 60);
		private SolidBrush m_CardBackgroundBrush = new SolidBrush(Color.Snow);

		public DeckViewModel() {
			UploadToImgurCommand = new SimpleCommand(UploadToImgur);
		}

		private string m_DeckName = null;
		public string DeckName {
			get => m_DeckName;
			set => SetProperty(ref m_DeckName, value);
		}

		public ObservableCollection<DeckCardViewModel> m_CardList = new ObservableCollection<DeckCardViewModel>();
		public ObservableCollection<DeckCardViewModel> CardList {
			get => m_CardList;
			set => SetProperty(ref m_CardList, value);
		}

		private bool m_IsPreviewImageTabOpen = false;
		public bool IsPreviewImageTabOpen {
			get => m_IsPreviewImageTabOpen;
			set {
				SetProperty(ref m_IsPreviewImageTabOpen, value);
				if (value) {
					Task.Run(DrawImage);
				}
			}
		}

		private BitmapSource m_PreviewDeckBitmap;
		public BitmapSource PreviewDeckBitmap {
			get => m_PreviewDeckBitmap;
			set => SetProperty(ref m_PreviewDeckBitmap, value);
		}

		private ICommand m_UploadToImgurCommand;
		public ICommand UploadToImgurCommand {
			get => m_UploadToImgurCommand;
			set => SetProperty(ref m_UploadToImgurCommand, value);
		}

		private string m_ImgurLink = "";
		public string ImgurLink {
			get => m_ImgurLink;
			set => SetProperty(ref m_ImgurLink, value);
		}

		private Task DrawImage() {
			using (Graphics gfx = Graphics.FromImage(m_ImageBitmap)) {
				gfx.SmoothingMode = SmoothingMode.AntiAlias;
				gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
				gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
				gfx.CompositingQuality = CompositingQuality.HighQuality;

				for (int row = 0; row < CARD_COUNT_VERTICAL; row++) {
					for (int column = 0; column < CARD_COUNT_HORIZONTAL; column++) {
						DrawCard(gfx, row, column);
					}
				}

				gfx.Flush();
			}

			Application.Current.Dispatcher.Invoke(() => {
				PreviewDeckBitmap = Imaging.CreateBitmapSourceFromHBitmap(m_ImageBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			});

			m_ImageBitmap.Save("test.png", System.Drawing.Imaging.ImageFormat.Png);
			return Task.CompletedTask;
		}

		private void DrawCard(Graphics gfx, int row, int column) {
			RectangleF cardRect = new RectangleF(
				CARD_WIDTH * column,
				CARD_HEIGHT * row,
				CARD_WIDTH,
				CARD_HEIGHT);

			Rectangle fullCardRect = new Rectangle((int)cardRect.X, (int)cardRect.Y, (int)cardRect.Width, (int)cardRect.Height);
			gfx.FillRectangle(m_CardBackgroundBrush, fullCardRect);
			gfx.DrawRectangle(new Pen(Color.Black, 4), fullCardRect);

			int cardIndex = column + (row * CARD_COUNT_HORIZONTAL);
			if (CardList.Count > cardIndex) {
				int margin = 16;
				RectangleF cardRectMargins = DrawingUtilities.AddMarginToRect(cardRect, margin);

				RectangleF titleRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y, cardRectMargins.Width, cardRectMargins.Height * 0.25f);
				RectangleF descriptionRegion = new RectangleF(cardRectMargins.X, cardRectMargins.Y + cardRectMargins.Height * 0.25f, cardRectMargins.Width, cardRectMargins.Height * 0.75f);

				DeckCardViewModel card = CardList[cardIndex];

				DrawingUtilities.WriteStringInRegion(gfx, card.Title, titleRegion, m_StartFont);
				DrawingUtilities.WriteStringInRegion(gfx, card.Description, descriptionRegion, m_StartFont);
			}
		}

		private void UploadToImgur(object o) {
			ImgurLink = "Uploading...";
			Task.Run(async () => {
				ImgurLink = await ImgurUtilities.UploadImage("test.png");
			 });
		}
	}
}