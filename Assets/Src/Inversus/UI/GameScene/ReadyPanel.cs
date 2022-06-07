using System.Collections;
using TMPro;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.UI.GameScene
{
    public class ReadyPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        private WaitForSeconds _wfs_1;
        private WaitForSeconds _wfs_0_1;

        private void Awake()
        {
            _wfs_1 = new WaitForSeconds(1);
            _wfs_0_1 = new WaitForSeconds(0.1f);

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
            yield return _wfs_0_1;
            _text.SetText("READY");
            yield return _wfs_1;
            _text.SetText("GO!");
            yield return _wfs_1;
            _text.SetText("");
            SetDisplay(false);
        }
    }
}
