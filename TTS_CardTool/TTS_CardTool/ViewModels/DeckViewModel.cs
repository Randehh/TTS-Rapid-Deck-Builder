using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TTS_CardTool.Renderers;
using TTS_CardTool.Utilities;

namespace TTS_CardTool.ViewModels {
	public class DeckViewModel : ViewModelBase, IProjectModule {
		public const string LOCAL_RENDER_FILE_NAME = "render.png";
		public const int CARD_COUNT_HORIZONTAL = 10;
		public const int CARD_COUNT_VERTICAL = 7;

		public string ModuleName => $"Deck_{DeckConfig.DisplayName}";

		private const int MAX_CARD_COUNT = 69;

		public int ImageWidth => DeckConfig.Width;
		public int ImageHeight => DeckConfig.Height;
		public float CardWidth => (float)ImageWidth / CARD_COUNT_HORIZONTAL;
		public float CardHeight => (float)ImageHeight / CARD_COUNT_VERTICAL;


		private DeckSettingsViewModel m_SettingsVM;
		public DeckSettingsViewModel SettingsVM => m_SettingsVM;

		private ICardRenderer m_CardRenderer;

		public DeckViewModel(DeckConfig config) {
			DeckConfig = config;

			m_CardRenderer = new WindowsCardRenderer();
			m_CardRenderer.OnCardRendered += (card) => SelectedCardBitmap = card;
			m_CardRenderer.OnDeckRendered += (deck) => PreviewDeckBitmap = deck;

			m_SettingsVM = new DeckSettingsViewModel(this);

			UploadToImgurCommand = new SimpleCommand(UploadToImgur);
			NewCardCommand = new SimpleCommand(AddNewCard, CanAddNewCard);
			RemoveCardCommand = new SimpleCommand(RemoveCard);

			CardList.CollectionChanged += OnCardListChanged;
		}

		private DeckConfig m_DeckConfig;
		public DeckConfig DeckConfig {
			get => m_DeckConfig;
			set {
				if (m_DeckConfig != null) m_DeckConfig.OnDeckUpdated -= DrawCardPreview;
				SetProperty(ref m_DeckConfig, value);
				DrawCardPreview();
				if (m_DeckConfig != null) m_DeckConfig.OnDeckUpdated += DrawCardPreview;
			}
		}

		public string CardCountStatus => $"{CardDisplayList.Count}/{MAX_CARD_COUNT}";

		private ObservableCollection<DeckCardViewModel> m_CardList = new ObservableCollection<DeckCardViewModel>();
		public ObservableCollection<DeckCardViewModel> CardList {
			get => m_CardList;
			set => SetProperty(ref m_CardList, value);
		}

		private List<IDeckCardViewModel> m_CardDisplayList = new List<IDeckCardViewModel>();
		public List<IDeckCardViewModel> CardDisplayList {
			get => m_CardDisplayList;
			set => SetProperty(ref m_CardDisplayList, value);
		}

		private IDeckCardViewModel m_SelectedCard;
		public IDeckCardViewModel SelectedCard {
			get => m_SelectedCard;
			set {
				if (m_SelectedCard != null) m_SelectedCard.OnCardUpdated -= DrawCardPreview;
				SetProperty(ref m_SelectedCard, value);
				DrawCardPreview();
				if (m_SelectedCard != null) m_SelectedCard.OnCardUpdated += DrawCardPreview;
			}
		}

		private ICommand m_NewCardCommand;
		public ICommand NewCardCommand {
			get => m_NewCardCommand;
			set => SetProperty(ref m_NewCardCommand, value);
		}

		private ICommand m_RemoveCardCommand;
		public ICommand RemoveCardCommand {
			get => m_RemoveCardCommand;
			set => SetProperty(ref m_RemoveCardCommand, value);
		}

		private bool m_IsPreviewImageTabOpen = false;
		public bool IsPreviewImageTabOpen {
			get => m_IsPreviewImageTabOpen;
			set {
				SetProperty(ref m_IsPreviewImageTabOpen, value);
				if (value) {
					DrawDeckPreview();
				}
			}
		}

		private BitmapSource m_PreviewDeckBitmap;
		public BitmapSource PreviewDeckBitmap {
			get => m_PreviewDeckBitmap;
			set => SetProperty(ref m_PreviewDeckBitmap, value);
		}

		private BitmapSource m_SelectedCardBitmap;
		public BitmapSource SelectedCardBitmap {
			get => m_SelectedCardBitmap;
			set => SetProperty(ref m_SelectedCardBitmap, value);
		}

		private bool m_IsRendering = false;
		public bool IsRendering {
			get => m_IsRendering;
			set => SetProperty(ref m_IsRendering, value);
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

		public void AddNewCard(object o) {
			DeckCardViewModel newCard = o == null ? new DeckCardViewModel() : o as DeckCardViewModel;
			m_CardList.Add(newCard);
			newCard.PropertyChanged += OnCardDataUpdated;
		}

		private bool CanAddNewCard() {
			return m_CardList.Count < MAX_CARD_COUNT;
		}

		private void RemoveCard(object o) {
			DeckCardViewModel card = o as DeckCardViewModel;
			card.PropertyChanged += OnCardDataUpdated;
			m_CardList.Remove(card);
		}

		private void OnCardDataUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			UpdateDisplayList();
		}

		private void OnCardListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdateDisplayList();
		}

		private void UpdateDisplayList() {
			List<IDeckCardViewModel> displayList = new List<IDeckCardViewModel>();
			foreach (DeckCardViewModel card in CardList) {
				displayList.Add(card);

				for (int i = 1; i < card.Count; i++) {
					displayList.Add(new DeckCardDisplayViewModel(card, true));
				}
			}
			CardDisplayList = displayList;
			OnPropertyChanged(nameof(CardCountStatus));
		}

		public void InvokeRenderers() {
			DrawDeckPreview();
			DrawCardPreview();
		}

		private void DrawDeckPreview() {
			Task.Run(async () => await m_CardRenderer.RenderDeck(this));
		}

		private void DrawCardPreview() {
			Task.Run(async () => await m_CardRenderer.RenderCard(this, SelectedCard));
		}

		private void UploadToImgur(object o) {
			Task.Run(async () => {
				ImgurLink = "Rendering...";
				DrawDeckPreview();

				ImgurLink = "Uploading...";
				string resultUrl = await ImgurUtilities.UploadImage(LOCAL_RENDER_FILE_NAME);
				if (string.IsNullOrWhiteSpace(resultUrl)) {
					ImgurLink = "Failed to upload.";
				} else {
					ImgurLink = resultUrl;
				}
			});
		}
	}
}