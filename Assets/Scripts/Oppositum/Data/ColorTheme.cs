using System;
using UnityEngine;

namespace Oppositum.Data
{
    [Serializable]
    public class ColorTheme
    {
        [SerializeField]
        private int _id;
        [SerializeField]
        private string _uiName;
        [SerializeField]
        private Color _side1Color;
        [SerializeField]
        private Color _side2Color;
        [SerializeField]
        private Color _backgroundColor;

        public int Id => _id;
        public string UiName => _uiName;
        public Color Side1Color => _side1Color;
        public Color Side2Color => _side2Color;
        public Color BackgroundColor => _backgroundColor;
    }
}
