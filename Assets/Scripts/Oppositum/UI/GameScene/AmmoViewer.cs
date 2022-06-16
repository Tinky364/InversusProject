using UnityEngine;
using UnityEngine.UI;

namespace Oppositum.UI.GameScene
{
    public class AmmoViewer : MonoBehaviour
    {
        private Image _img;

        private void Awake()
        {
            _img = GetComponent<Image>();
        }

        public void Initialize(Color color)
        {
            SetColor(color);
        }

        private void SetColor(Color color)
        {
            _img.color = color;
        }
        
        public void CalculateFillAmount(float current, float max)
        {
            float fillAmount = current / max;
            _img.fillAmount = fillAmount;
        }
    }
}
