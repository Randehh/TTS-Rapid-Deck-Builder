using System;
using System.Collections.Generic;

namespace TTS_CardTool.ViewModels {
	public interface IDeckCardViewModel {
		Dictionary<string, string> CardValues { get; set; }
		int Count { get; set; }
		Action OnCardUpdated { get; set; }
	}
}