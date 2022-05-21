using System;
using UnityEngine;

namespace Inversus.Data
{
    [Serializable]
    public class ColorTheme
    {
        [SerializeField]
        private int _id;
        [SerializeField]
        private Color _side1Color;
        [SerializeField]
        private Color _side2Color;
        [SerializeField]
        private Color _backgroundColor;

        public int Id => _id;
        public Color Side1Color => _side1Color;
        public Color Side2Color => _side2Color;
        public Color BackgroundColor => _backgroundColor;
    }
}
