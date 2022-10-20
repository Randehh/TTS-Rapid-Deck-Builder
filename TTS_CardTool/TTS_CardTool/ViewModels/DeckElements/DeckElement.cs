using RondoFramework.BaseClasses;

namespace TTS_CardTool.ViewModels.DeckElements
{
    public abstract class DeckElement : ViewModelBase
    {
        private string m_DisplayName;
        public string DisplayName
        {
            get => m_DisplayName;
            set => SetProperty(ref m_DisplayName, value);
        }

        private string m_DisplayValue;
        public string DisplayValue
        {
            get => m_DisplayValue;
            set => SetProperty(ref m_DisplayValue, value);
        }

        private bool m_IsStatic = false;
        public bool IsStatic
        {
            get => m_IsStatic;
            set => SetProperty(ref m_IsStatic, value);
        }

        private float m_PositionStartX;
        public float PositionStartX
        {
            get => m_PositionStartX;
            set => SetProperty(ref m_PositionStartX, value);
        }

        private float m_PositionStartY;
        public float PositionStartY
        {
            get => m_PositionStartY;
            set => SetProperty(ref m_PositionStartY, value);
        }

        private float m_PositionEndX;
        public float PositionEndX
        {
            get => m_PositionEndX;
            set => SetProperty(ref m_PositionEndX, value);
        }

        public float m_PositionEndY;
        public float PositionEndY
        {
            get => m_PositionEndY;
            set => SetProperty(ref m_PositionEndY, value);
        }

        public abstract DeckElement Copy();
    }
}