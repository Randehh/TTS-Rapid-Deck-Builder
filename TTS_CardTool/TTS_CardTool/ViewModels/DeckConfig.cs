using RondoFramework.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TTS_CardTool.ViewModels.DeckElements;

namespace TTS_CardTool.ViewModels
{
	public class DeckConfig : ViewModelBase
	{

		private static List<DeckElement> m_StandardDeckElements = new List<DeckElement>() {
			new DeckElementText() {
				DisplayName = "Title",
				PositionStartX = 0.1f,
				PositionEndX = 0.9f,
				PositionStartY = 0.1f,
				PositionEndY = 0.3f,
			},
			new DeckElementText() {
				DisplayName = "Description",
				PositionStartX = 0.1f,
				PositionEndX = 0.9f,
				PositionStartY = 0.4f,
				PositionEndY = 0.9f,
			},
		};
		private static List<DeckConfig> m_CachedList;
		public static List<DeckConfig> GetStandardDeckConfigs() {
			if (m_CachedList == null) {
				m_CachedList = new List<DeckConfig>() {
					new DeckConfig() { DisplayName = "Rectangle (4096 x 4096)", Height = 4096, Width = 4096, Elements = new ObservableCollection<DeckElement>(m_StandardDeckElements) },
					new DeckConfig() { DisplayName = "Rectangle (2048 x 2048)", Height = 2048, Width = 2048 , Elements = new ObservableCollection<DeckElement>(m_StandardDeckElements) } ,
					new DeckConfig() { DisplayName = "Square (2884 x 4096)", Height = 2884, Width = 4096, Elements = new ObservableCollection<DeckElement>(m_StandardDeckElements)  },
					new DeckConfig() { DisplayName = "Square (1442 x 2048)", Height = 1442, Width = 2048, Elements = new ObservableCollection<DeckElement>(m_StandardDeckElements) },
				};
			}
			return m_CachedList;
		}

		public Action OnDeckUpdated { get; set; } = delegate { };

		private string m_DisplayName;
		public string DisplayName
		{
			get => m_DisplayName;
			set => SetProperty(ref m_DisplayName, value);
		}

		private int m_Width;
		public int Width
		{
			get => m_Width;
			set => SetProperty(ref m_Width, value);
		}

		private int m_Height;
		public int Height
		{
			get => m_Height;
			set => SetProperty(ref m_Height, value);
		}

		public bool HasCustomBackground => !string.IsNullOrWhiteSpace(CustomBackground);

		private string m_CustomBackground = "";
		public string CustomBackground
		{
			get => m_CustomBackground;
			set
			{
				SetProperty(ref m_CustomBackground, value);
				OnDeckUpdated();
			}
		}

		private string m_Font = "Arial";
		public string Font
		{
			get => m_Font;
			set
			{
				SetProperty(ref m_Font, value);
				OnDeckUpdated();
			}
		}

		private int m_OutlineSize = 5;
		public int OutlineSize
		{
			get => m_OutlineSize;
			set
			{
				SetProperty(ref m_OutlineSize, value);
				OnDeckUpdated();
			}
		}

		private ObservableCollection<DeckElement> m_Elements = new ObservableCollection<DeckElement>();
		public ObservableCollection<DeckElement> Elements
		{
			get => m_Elements;
			set
			{
				if (m_Elements != null) {
					m_Elements.CollectionChanged -= OnElementsListUpdated;
				}
				SetProperty(ref m_Elements, value);
				if (value != null) {
					value.CollectionChanged += OnElementsListUpdated;
				}
			}
		}

		private void OnElementsListUpdated(object sender, NotifyCollectionChangedEventArgs e) {
			OnDeckUpdated();
		}

		public DeckConfig Copy() {
			DeckConfig config = new DeckConfig() {
				DisplayName = DisplayName,
				Width = Width,
				Height = Height,
				Font = Font,
				OutlineSize = OutlineSize,
				CustomBackground = CustomBackground,
			};

			foreach(DeckElement elem in Elements) {
				config.Elements.Add(elem.Copy());
            }
			return config;
		}
	}
}