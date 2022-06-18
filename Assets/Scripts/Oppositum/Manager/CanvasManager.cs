using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Helper;
using Oppositum.UI;
using static Oppositum.Facade;

namespace Oppositum.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private Panel _backgroundPanel;
        [SerializeField]
        private Panel _foregroundPanel;
        [Header("AUDIO"), SerializeField, Expandable]
        private AudioData _buttonSelectAudioData;
        [SerializeField]
        private AudioData _buttonClickAudioData;

        public Panel BackgroundPanel => _backgroundPanel;
        public Panel ForegroundPanel => _foregroundPanel;
        public GameObject LastSelectedGameObject => _lastSelectedGameObject;

        private AudioSource _audioSource;
        private InputSystemUIInputModule _inputSystemUIInputModule;
        private GameObject _lastSelectedGameObject;
        
        protected override void Awake()
        {
            base.Awake();

            _audioSource = GetComponent<AudioSource>();
            _inputSystemUIInputModule = _eventSystem.GetComponent<InputSystemUIInputModule>();
        }

        public void SetUiInputModule(PlayerInput playerInput)
        {
            playerInput.uiInputModule = _inputSystemUIInputModule;
        }

        public void SetSelectedGameObject(GameObject element)
        {
            if (_eventSystem == null) return;
            _audioSource.enabled = false;
            _eventSystem.SetSelectedGameObject(element);
            _audioSource.enabled = true;
        }
        
        public void LoadScene(SceneData targetSceneData)
        {
            SSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
        
        public void SetUiInput(bool isActive)
        {
            if (isActive)
            {
               _eventSystem.SetSelectedGameObject(_lastSelectedGameObject);
                _foregroundPanel.SetDisplay(false);
            }
            else
            {
                _lastSelectedGameObject = _eventSystem.currentSelectedGameObject;
                _eventSystem.SetSelectedGameObject(null);
                _foregroundPanel.SetDisplay(true);
            }
        }

        public void PlayButtonSelectAudio()
        {
            if (!_audioSource.enabled) return;
            _buttonSelectAudioData.Play(_audioSource);
        }

        public void PlayButtonClickAudio()
        {
            if (!_audioSource.enabled) return;
            _buttonClickAudioData.Play(_audioSource);
        }
    }
}
