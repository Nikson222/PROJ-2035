using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Constants;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class AudioInstaller : MonoInstaller
    {
        [SerializeField] private AudioConfig _audioConfig;

        public override void InstallBindings()
        {
            if (_audioConfig == null)
            {
                Debug.LogError("AudioConfig not assigned in AudioInstaller!");
                return;
            }

            Container.BindInstance(_audioConfig);
            
            var parent = new GameObject("AudioSources");

            var soundSourceObject = new GameObject(NameConstants._soundSourceName);
            var musicSourceObject = new GameObject(NameConstants._musicSourceName);

            soundSourceObject.transform.parent = parent.transform;
            musicSourceObject.transform.parent = parent.transform;

            var soundSource = soundSourceObject.AddComponent<AudioSource>();
            var musicSource = musicSourceObject.AddComponent<AudioSource>();
            
            Container.BindInstance(parent).AsCached();
            Container.Bind<AudioSource>().WithId(NameConstants._soundSourceName).FromInstance(soundSource).AsCached();
            Container.Bind<AudioSource>().WithId(NameConstants._musicSourceName).FromInstance(musicSource).AsCached();

            musicSource.loop = true;
            
            DontDestroyOnLoad(parent);

            Container.BindInterfacesAndSelfTo<AudioService>().AsSingle().NonLazy();
        }
    }
}