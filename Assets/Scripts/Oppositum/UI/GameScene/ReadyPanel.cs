using System.Collections;
using TMPro;
using UnityEngine;
using Oppositum.Attribute;
using Oppositum.Data;
using static Oppositum.Facade;

namespace Oppositum.UI.GameScene
{
    public class ReadyPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField, Expandable]
        private AudioData _readyGoAudioData;

        private AudioSource _audioSource;
        private WaitForSeconds _wfs_1;
        private WaitForSeconds _wfs_1_6;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _wfs_1 = new WaitForSeconds(1f);
            _wfs_1_6 = new WaitForSeconds(1.6f);

            SEventBus.RoundCreated.AddListener(DisplayReadyPanel);
        }

        private void OnDestroy()
        {
            SEventBus.RoundCreated.RemoveListener(DisplayReadyPanel);
        }

        private void DisplayReadyPanel()
        {
            SetDisplay(true);
            SGameCanvasManager.ForegroundPanel.SetDisplay(false);
            StartCoroutine(DisplayReadyPanelCor());
        }

        private IEnumerator DisplayReadyPanelCor()
        {
            _readyGoAudioData.Play(_audioSource);
            _text.SetText("READY");
            _text.color = SDatabase.GetColorTheme(1).Side1Color;
            yield return _wfs_1_6;
            _text.SetText("GO!");
            _text.color = SDatabase.GetColorTheme(1).Side2Color;
            yield return _wfs_1;
            _text.SetText("");
            SetDisplay(false);
        }
    }
}
