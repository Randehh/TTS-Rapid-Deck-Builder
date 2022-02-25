using RondoFramework.BaseClasses;
using System;

namespace TTS_CardTool.ViewModels {
	public class DeckCardDisplayViewModel : ViewModelBase, IDeckCardViewModel {

		public DeckCardDisplayViewModel(DeckCardViewModel referenceCard, bool isChild) {
			m_ReferenceCard = referenceCard;
			IsChild = isChild;

			referenceCard.PropertyChanged += ReferenceCard_PropertyChanged;
		}

		~DeckCardDisplayViewModel() {
			m_ReferenceCard.PropertyChanged -= ReferenceCard_PropertyChanged;
		}

		private void ReferenceCard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			OnPropertyChanged(e.PropertyName);
		}

		private DeckCardViewModel m_ReferenceCard;

		public string Title {
			get => m_ReferenceCard.Title;
			set => m_ReferenceCard.Title = value;
		}

		public string Description {
			get => m_ReferenceCard.Description;
			set => m_ReferenceCard.Description = value;
		}

		public int Count {
			get => m_ReferenceCard.Count;
			set => m_ReferenceCard.Count = value;
		}

		public bool IsChild { get; }

		public Action OnCardUpdated {
			get => m_ReferenceCard.OnCardUpdated;
			set => m_ReferenceCard.OnCardUpdated = value;
		}
	}
}