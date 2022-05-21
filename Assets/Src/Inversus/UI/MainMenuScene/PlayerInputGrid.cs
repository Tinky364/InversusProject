using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inversus.UI
{
    public class PlayerInputGrid : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _deviceText;
        [SerializeField]
        private Image _info;

        public void Display(string username, string inputDevice)
        {
            _nameText.SetText($"Name: {username}");
            _deviceText.SetText($"Device: {inputDevice}");
            _nameText.gameObject.SetActive(true);
            _deviceText.gameObject.SetActive(true);
            _info.gameObject.SetActive(false);
        }

        public void Hide()
        {
            _nameText.SetText("Name:");
            _deviceText.SetText("Device:");
            _nameText.gameObject.SetActive(false);
            _deviceText.gameObject.SetActive(false);
            _info.gameObject.SetActive(true);
        }
    }
}
