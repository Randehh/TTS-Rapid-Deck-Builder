namespace TTS_CardTool.ViewModels.DeckElements
{
    public class DeckElementText : DeckElement
    {
        private string m_Text;
        public string Text
        {
            get => m_Text;
            set => SetProperty(ref m_Text, value);
        }

        public override DeckElement Copy() {
            return new DeckElementText() {
                DisplayName = DisplayName,
                DisplayValue = DisplayValue,
                IsStatic = IsStatic,
                PositionStartX = PositionStartX,
                PositionEndX = PositionEndX,
                PositionStartY = PositionStartY,
                PositionEndY = PositionEndY,
                Text = Text,
            };
        }
    }
}
