using System;
using UnityEngine;
using UnityEngine.UI;

namespace Inversus.UI
{
    public class ProgressBar : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private Sprite _backgroundSprite;
        [SerializeField]
        private Sprite _foregroundSprite;
        [SerializeField, Range(0, 1)]
        private float _fillAmount = 1;
        [SerializeField, Min(0)]
        private float _speedOfLerpAnimation = 1f;
        [SerializeField]
        private FillMethod _fillMethod = FillMethod.Horizontal;
        [SerializeField, HideInInspector]
        private HorizontalFillOrigin _horizontalFillOrigin = HorizontalFillOrigin.Left;
        [SerializeField, HideInInspector]
        private VerticalFillOrigin _verticalFillOrigin = VerticalFillOrigin.Bottom;
        
        public enum FillMethod { Horizontal, Vertical }
        private enum HorizontalFillOrigin { Left, Right }
        private enum VerticalFillOrigin { Bottom, Top }

        private event Action FillAmountChanged;

        private float FillAmount
        {
            get => _fillAmount;
            set
            {
                _fillAmount = value;
                if (_foreground != null)
                    _foreground.fillAmount = _fillAmount;
            }
        }
        private float _targetFillAmount;
        private bool _isAmountChanged;
        private Image _foreground;
        private Image _background;

        private void Awake()
        {
            _foreground = transform.Find("Foreground").GetComponent<Image>();
            _targetFillAmount = FillAmount;
        }
        
        private void OnEnable()
        {
            FillAmountChanged += StartFillAmountLerp;
        }

        private void OnDisable()
        {
            FillAmountChanged -= StartFillAmountLerp;
        }

        private void Update()
        {
            FillAmountLerp();
        }
        
        public void SetAmount(float maxValue, float value, bool direct)
        {
            float normalizedValue = value / maxValue;
            if (normalizedValue < 0)
                return;
            if (direct)
            {
                FillAmount = normalizedValue;
                _targetFillAmount = normalizedValue;
            }
            else
            {
                _targetFillAmount = normalizedValue;
                FillAmountChanged?.Invoke();
            }
        }
        
        public void SetAmount(int maxValue, int value)
        {
            float normalizedValue = (float) value / maxValue;
            if (normalizedValue < 0)
                return;
            _targetFillAmount = normalizedValue;
            FillAmountChanged?.Invoke();
        }

        private void StartFillAmountLerp()
        {
            _isAmountChanged = true;
        }

        private void FillAmountLerp()
        {
            if (!_isAmountChanged) return;

            if (Mathf.Abs(_targetFillAmount - FillAmount) > 0.001f)
            {
                if (_targetFillAmount - FillAmount > 0)
                {
                    FillAmount += _speedOfLerpAnimation * Time.deltaTime;
                }
                else
                {
                    FillAmount -= _speedOfLerpAnimation * Time.deltaTime;
                }
            }
            else
            {
                _isAmountChanged = false;
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_foreground == null)
                _foreground = transform.Find("Foreground").GetComponent<Image>();
            _foreground.sprite = _foregroundSprite;
            _foreground.fillAmount = _fillAmount;
            switch (_fillMethod)
            {
                case FillMethod.Horizontal:
                    _foreground.fillMethod = Image.FillMethod.Horizontal;
                    _foreground.fillOrigin = (int)_horizontalFillOrigin;
                    break;
                case FillMethod.Vertical:
                    _foreground.fillMethod = Image.FillMethod.Vertical;
                    _foreground.fillOrigin = (int)_verticalFillOrigin;
                    break;
            }

            if (_background == null)
                _background = transform.Find("Background").GetComponent<Image>();
            _background.sprite = _backgroundSprite;
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
