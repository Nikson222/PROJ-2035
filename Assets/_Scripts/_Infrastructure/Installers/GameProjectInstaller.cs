using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game;
using _Scripts.Game.Data;
using _Scripts.Game.Progress;
using LoadingCurtain;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class GameProjectInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunnerPrefab;
        [SerializeField] private Canvas _curtainCanvas;

        [SerializeField] private bool _isCoroutineRunnerRequired;
        [SerializeField] private bool _isCurtainRequired;
        [SerializeField] private bool _isAudioManagerRequired;
        [SerializeField] private bool _isPopupTextServiceRequired;
        
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private LevelCard _levelCardPrefab;
        
        public override void InstallBindings()
        {
            InstantiateAndBindCoroutineRunner();
            InstantiateAndBindCurtain();
            
            Container.BindMemoryPool<LevelCard, LevelCard.Pool>()
                .WithInitialSize(4)
                .FromComponentInNewPrefab(_levelCardPrefab)
                .UnderTransformGroup("LevelCards");
            
            Container.BindInterfacesAndSelfTo<ProgressManager>().FromNew().AsSingle();
            Container.Bind<LevelDatabase>().FromInstance(_levelDatabase).AsSingle();
            
            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerProfile>().AsSingle();
        }

        private void InstantiateAndBindCoroutineRunner()
        {
            var coroutineRunner = Container.InstantiatePrefabForComponent<CoroutineRunner>(_coroutineRunnerPrefab);
            Container.BindInterfacesAndSelfTo<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();
        }

        private void InstantiateAndBindCurtain()
        {
            var curtainCanvas = Container.InstantiatePrefab(_curtainCanvas);
            var curtain = curtainCanvas.GetComponentInChildren<Curtain>();
            
            Container.BindInterfacesAndSelfTo<Curtain>().FromInstance(curtain).AsSingle();
        }
    }
}