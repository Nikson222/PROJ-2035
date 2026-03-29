using _Scripts.Game;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private AssetProvider _assetProvider;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private DotColorConfig _colorConfig;
        [SerializeField] private LevelTimerController _levelTimerController; 
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelTimerController>().FromInstance(_levelTimerController).AsSingle();
            Container.BindInterfacesAndSelfTo<LevelProgressChecker>().FromNew().AsSingle();
            Container.Bind<DotColorConfig>().FromInstance(_colorConfig).AsSingle();
            Container.Bind<AssetProvider>().FromInstance(_assetProvider).AsSingle();
            Container.Bind<GridManager>().FromInstance(_gridManager).AsSingle();
        }
    }
}