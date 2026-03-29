using System;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Constants;
using _Scripts._Infrastructure.Data;
using Core.Infrastructure.SaveLoad;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Scripts._Infrastructure.Services
{
    public class AudioService : ISavable, IInitializable
    {
        private readonly AudioSource _soundSource;
        private readonly AudioSource _musicSource;

        private readonly AudioConfig _config;
        
        private float _savedMusicVolume;
        private float _savedSoundVolume;
        
        private float _lastMusicVolume;

        public float SoundVolume
        {
            get => _savedSoundVolume;
            set
            {
                _soundSource.volume = value;
                _savedSoundVolume = value;
            }
        }

        public float MusicVolume
        {
            get => _savedMusicVolume;
            set
            {
                _musicSource.volume = value;
                _savedMusicVolume = value;
            }
        }


        public Type DataType => typeof(AudioData);

        public string SavePath => SavePathConstants.AudioSavePath;

        public AudioService([Inject(Id = NameConstants._soundSourceName)] AudioSource soundSource, 
            [Inject(Id = NameConstants._musicSourceName)] AudioSource musicSource, AudioConfig config)
        {
            _soundSource = soundSource;
            _musicSource = musicSource;
            
            _config = config;
        }


        public void Initialize()
        {
            if (_lastMusicVolume == 0)
                _lastMusicVolume = _musicSource.volume;
        }

        public void PlaySound(SoundType type)
        {
            var clips = _config.Sounds.FindAll(x => x.Type == type);

            if (clips.Count == 0)
            {
                Debug.Log("Sound with type " + type + " not found");
                return;
            }

            var clip = clips[Random.Range(0, clips.Count)].Clip;

            _soundSource.PlayOneShot(clip);
        }

        public void PlayMusic(bool withFade = true)
        {
            if (_config.MusicClips.Count == 0)
            {
                Debug.Log("Music not found");
                return;
            }

            var clip = _config.MusicClips[Random.Range(0, _config.MusicClips.Count)];
            _musicSource.clip = clip;

            _musicSource.Play();
            
            if (withFade)
            {
                _musicSource.volume = 0;
                
                Sequence sequence = DOTween.Sequence();

                sequence.Append(DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, _savedMusicVolume,
                        1f)).SetEase(Ease.InOutSine);
            }
        }

        public void StopMusic(bool withFade = true)
        {
            if (!withFade)
            {
                _musicSource.Stop();
                return;
            }
            
            Sequence sequence = DOTween.Sequence();

            sequence.Append(DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, 0, 1f))
                .SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _musicSource.Stop();
                    _musicSource.volume = _savedMusicVolume;
                });
        }

        
        public void MuteMusic() => _musicSource.mute = true;

        public void UnMuteMusic() => _musicSource.mute = false;

        public void MuteSound() => _soundSource.mute = true;

        public void UnMuteSound() => _soundSource.mute = false;
        
        
        public object GetData()
        {
            if (!_musicSource || !_soundSource)
            {
                Debug.Log($"Audio sources not found. Saved recovery data {_savedMusicVolume}, ${_savedSoundVolume}");
                return new AudioData(_savedMusicVolume, _savedSoundVolume);
            }
            
            return new AudioData(_musicSource.volume, _soundSource.volume);
        }

        public void SetData(object data)
        {
            if (data is not AudioData audioData) return;
            
            MusicVolume = audioData.MusicVolume;
            SoundVolume = audioData.SoundVolume;
        }

        public void SetInitialData()
        {
            MusicVolume = _config.StartMusicVolume;
            SoundVolume = _config.StartSoundVolume;
        }
    }
}