using UnityEngine;

namespace Oppositum.Data
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Oppositum/Audio Data", order = 1)]
    public class AudioData : ScriptableObject
    {
        [SerializeField]
        private AudioClip _clip;

        [Header("VOLUME"), SerializeField, Range(0, 1)]
        private float _defaultVolume = 0.5f;
        [SerializeField, Range(0, 1)]
        private float _minVolume = 0f;
        [SerializeField, Range(0, 1)] 
        private float _maxVolume = 1f;
        [Header("PITCH"), SerializeField, Range(0, 3)]
        private float _defaultPitch = 1f;
        [SerializeField, Range(0, 3)] 
        private float _minPitch = 0f;
        [SerializeField, Range(0, 3)] 
        private float _maxPitch = 3f;
        
        public void Play(AudioSource source, bool randomize = false)
        {
            source.clip = _clip;
            source.volume = _defaultVolume;
            source.pitch = _defaultPitch;
            if (randomize)
            {
                source.volume = Random.Range(_minVolume, _maxVolume);
                source.pitch = Random.Range(_minPitch, _maxPitch);
            }
            source.Play();
        }
        
        public void Play(float volume, AudioSource source,  float pitch)
        {
            source.clip = _clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
        
        public void Play(float volume, AudioSource source, bool randomizePitch = false)
        {
            source.clip = _clip;
            source.volume = volume;
            source.pitch = _defaultPitch;
            if (randomizePitch) source.pitch = Random.Range(_minPitch, _maxPitch);
            source.Play();
        }
        
        public void Play(AudioSource source, float pitch, bool randomizeVolume = false)
        {
            source.clip = _clip;
            source.volume = _defaultVolume;
            if (randomizeVolume) source.volume = Random.Range(_minVolume, _maxVolume);
            source.pitch = pitch;
            source.Play();
        }
    }
}
