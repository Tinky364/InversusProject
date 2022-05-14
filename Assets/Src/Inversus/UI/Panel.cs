using UnityEngine;
using UnityEngine.Events;

using Inversus.Attribute;

namespace Inversus.UI
{
    public class Panel : MonoBehaviour
    {
        [ReadOnly] 
        public UnityEvent Displayed;
        [ReadOnly] 
        public UnityEvent Hided;

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
