using UnityEngine;
using UnityEngine.Events;

namespace Inversus.UI
{
    public class Panel : MonoBehaviour
    {
        public UnityEvent Displayed = new();
        public UnityEvent Hided = new();
        
        public void SetDisplay(bool value)
        {
            if (value)
            {
                Displayed?.Invoke();
            }
            else
            {
                Hided?.Invoke();
            }
            gameObject.SetActive(value);
        }
    }
}
