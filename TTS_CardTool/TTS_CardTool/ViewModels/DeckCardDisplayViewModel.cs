using RondoFramework.BaseClasses;
using System;
using System.Collections.Generic;

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

		public Dictionary<string, string> CardValues {
			get => m_ReferenceCard.CardValues;
			set => m_ReferenceCard.CardValues = value;
		}

		public int Count {
			get => m_ReferenceCard.Count;
			set => m_ReferenceCard.Count = value;
		}

		public bool IsChild { get; }

		public Action OnCardUpdated
		{
			get => m_ReferenceCard.OnCardUpdated;
			set => m_ReferenceCard.OnCardUpdated = value;
		}
    }
}