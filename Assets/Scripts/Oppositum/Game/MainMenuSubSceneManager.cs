using UnityEngine;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Manager;
using static Oppositum.Facade;

namespace Oppositum.Game
{
    public class MainMenuSubSceneManager : SubSceneManager
    {
        [SerializeField, Expandable]
        private AudioData _mainMenuAudioData;

        private AudioSource _audioSource;

        protected override void Awake()
        {
            base.Awake();

            _audioSource = GetComponent<AudioSource>();
        }

        protected override void OnSceneLoaded(SceneData sceneData)
        {
            SMainManager.State = States.MainMenu;
            SInputProfileManager.Disable();
            _mainMenuAudioData.Play(_audioSource);
        }

        public void Quit()
        {
            SMainManager.Quit();
        }
    }
}

