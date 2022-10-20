using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TTS_CardTool.Renderers;
using TTS_CardTool.Utilities;
using TTS_CardTool.ViewModels.DeckElements;

namespace TTS_CardTool.ViewModels {
	public class DeckViewModel : ViewModelBase, IProjectModule {
		public const string RENDER_FOLDER = "DeckRenders";
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

		private string m_UploadFileName;

		public DataGrid DeckGridControl { get; set; }

		public DeckViewModel(DeckConfig config) {
			m_CardRenderer = new WindowsCardRenderer();
			m_CardRenderer.OnCardRendered += (card) => SelectedCardBitmap = card;
			m_CardRenderer.OnDeckRendered += OnDeckRendered;

			DeckConfig = config;

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
				if (m_DeckConfig != null) m_DeckConfig.OnDeckUpdated -= OnDeckUpdated;
				SetProperty(ref m_DeckConfig, value);
				OnDeckUpdated();
				if (m_DeckConfig != null) m_DeckConfig.OnDeckUpdated += OnDeckUpdated;
			}
		}

		public string CardCountStatus => $"{CardDisplayList.Count}/{MAX_CARD_COUNT}";

		private ObservableCollection<DeckCardViewModel> m_CardList = new ObservableCollection<DeckCardViewModel>();
		public ObservableCollection<DeckCardViewModel> CardList {
			get => m_CardList;
			set => SetProperty(ref m_CardList, value);
		}

		private ObservableCollection<IDeckCardViewModel> m_CardDisplayList = new ObservableCollection<IDeckCardViewModel>();
		public ObservableCollection<IDeckCardViewModel> CardDisplayList {
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

		private void OnDeckUpdated() {
			GenerateGridColumns();
			DrawCardPreview();
        }

		public void GenerateGridColumns() {
			if (DeckGridControl == null) return;

			for(int i = DeckGridControl.Columns.Count -1; i >= 0; i--) {
				DataGridColumn column = DeckGridControl.Columns[i];
				if(!(column is DataGridTemplateColumn)) {
					DeckGridControl.Columns.Remove(column);
                }
            }

			for(int i = 0; i < DeckConfig.Elements.Count; i++) {
				DeckElement columnDef = DeckConfig.Elements[i];
				if (columnDef.IsStatic) continue;

				DeckGridControl.Columns.Insert(0, new DataGridTextColumn() {
					Header = columnDef.DisplayName,
					Binding = new Binding($"CardValues[{columnDef.DisplayName}]"),
				});
			}
		}

		public void AddNewCard(object o) {
			DeckCardViewModel newCard = o == null ? new DeckCardViewModel() : o as DeckCardViewModel;
			m_CardList.Add(newCard);
			newCard.OnCountUpdated += OnCardCountUpdated;
		}

		private bool CanAddNewCard() {
			return m_CardList.Count < MAX_CARD_COUNT;
		}

		private void RemoveCard(object o) {
			DeckCardViewModel card = o as DeckCardViewModel;
			card.OnCountUpdated -= OnCardCountUpdated;
			m_CardList.Remove(card);
		}

		private void OnCardCountUpdated(DeckCardViewModel card, int oldValue, int newValue) {
			int index = CardDisplayList.IndexOf(card);
			if(oldValue > newValue) {
				CardDisplayList.RemoveAt(index + 1);
			} else {
				CardDisplayList.Insert(index + 1, new DeckCardDisplayViewModel(card, true));
			}
		}

		private void OnCardListChanged(object sender, NotifyCollectionChangedEventArgs e) {
			DeckCardViewModel card;
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					card = e.NewItems[0] as DeckCardViewModel;
					CardDisplayList.Add(card);
					for(int i = 0; i < card.Count - 1; i++) {
						DeckCardDisplayViewModel vm = new DeckCardDisplayViewModel(card, true);
						CardDisplayList.Add(vm);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					card = e.OldItems[0] as DeckCardViewModel;
					CardDisplayList.Remove(card);
					while(CardDisplayList.Count > e.OldStartingIndex && CardDisplayList[e.OldStartingIndex] is DeckCardDisplayViewModel displayViewModel) {
						CardDisplayList.Remove(displayViewModel);
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					CardDisplayList.Clear();
					break;

			}
			OnPropertyChanged(nameof(CardCountStatus));
		}

		public void InvokeRenderers() {
			DrawDeckPreview();
			DrawCardPreview();
		}

		private void DrawDeckPreview() {
			m_CardRenderer.RenderDeck(this);
		}

		private void DrawCardPreview() {
			m_CardRenderer.RenderCard(this, SelectedCard);
		}

		private void OnDeckRendered(BitmapSource bitmapSource) {
			PreviewDeckBitmap = bitmapSource;

			if (!string.IsNullOrWhiteSpace(m_UploadFileName) && File.Exists(m_UploadFileName)) {
				string fileName = m_UploadFileName;
				m_UploadFileName = "";
				ImgurLink = "Uploading...";
				Task.Run(async () => {
					string resultUrl = await ImgurUtilities.UploadImage(fileName);
					if (string.IsNullOrWhiteSpace(resultUrl)) {
						ImgurLink = "Failed to upload.";
					} else {
						ImgurLink = resultUrl;
					}
				});
			}
		}

		private void UploadToImgur(object o) {
			if (!Directory.Exists(RENDER_FOLDER)) {
				Directory.CreateDirectory(RENDER_FOLDER);
			}
			ImgurLink = "Rendering...";
			m_UploadFileName = Path.Combine(RENDER_FOLDER, $"{Guid.NewGuid()}.png");
			m_CardRenderer.RenderDeck(this, m_UploadFileName);
		}
	}
}