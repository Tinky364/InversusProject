using TMPro;
using UnityEngine;

namespace Inversus.UI
{
    public class PlayerInputGrid : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _deviceText;

        public void Set(string username)
        {
            _nameText.text = $"Name: {username}";
            //_deviceText.text = $"Device: {device}";
        }
    }
}
