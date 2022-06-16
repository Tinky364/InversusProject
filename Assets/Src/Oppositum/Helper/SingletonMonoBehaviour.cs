using UnityEngine;

namespace Oppositum.Helper
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            SetSingleton();
        }

        private void SetSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("An instance of this singleton already exists.");
                Debug.LogWarning($"Destroyed Component => {this}");
                Destroy(this);
            }
            else
            {
                Instance = (T)this;
            }
        }
    }
}
