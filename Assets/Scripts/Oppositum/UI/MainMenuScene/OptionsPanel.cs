using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Oppositum.Facade;

namespace Oppositum.UI.MainMenuScene
{
    public class OptionsPanel : Panel
    {
        [SerializeField]
        private TMP_InputField _onlineNicknameInputField;
        [SerializeField]
        private Slider _volumeSlider;
        [SerializeField]
        private TMP_Dropdown _resolutionDropdown;
        [SerializeField]
        private TMP_Dropdown _displayModeDropdown;

        private void Awake()
        {
            _resolutionDropdown.ClearOptions();
            _displayModeDropdown.ClearOptions();
            
            _resolutionDropdown.AddOptions(SDatabase.GetResolutionNames());
            _displayModeDropdown.AddOptions(SDatabase.GetDisplayModeNames());
        }

        private void OnEnable()
        {
            _onlineNicknameInputField.text = SOnlineManager.NickName;
            _volumeSlider.value = SSettingManager.Volume;
            _resolutionDropdown.SetValueWithoutNotify(SSettingManager.ResolutionIndex);
            _displayModeDropdown.SetValueWithoutNotify(SSettingManager.DisplayModeIndex);
        }

        public void SaveButton_Click()
        {
            SOnlineManager.NickName = _onlineNicknameInputField.text;
            SSettingManager.SetVolume(_volumeSlider.value);
            SSettingManager.SetResolution(_resolutionDropdown.value);
            SSettingManager.SetDisplayMode(_displayModeDropdown.value);
            
            SMainMenuCanvasManager.MainMenuPanel.SetDisplay(true);
            SMainMenuCanvasManager.OptionsPanel.SetDisplay(false);
        }
    }
}
