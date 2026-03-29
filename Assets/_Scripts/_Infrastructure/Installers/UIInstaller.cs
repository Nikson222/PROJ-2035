using UnityEngine;
using Zenject;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.UI;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private GameObject _canvasPrefab;
    [SerializeField] private UIPanelConfig _uiPanelConfig;
    
    public override void InstallBindings()
    {
        // Биндим конфигурацию UI
        Container.Bind<UIPanelConfig>().FromInstance(_uiPanelConfig).AsSingle();

        // Инстанцируем Canvas из префаба
        Canvas canvasInstance = Container.InstantiatePrefabForComponent<Canvas>(_canvasPrefab);
        Object.DontDestroyOnLoad(canvasInstance.gameObject);

        // Регистрируем Canvas с идентификатором "UICanvas"
        Container.Bind<Canvas>().WithId("UICanvas").FromInstance(canvasInstance).AsSingle();
        
        // Биндим UIPanelService, который получает Canvas через [Inject(Id = "UICanvas")]
        Container.Bind<UIPanelService>()
            .AsSingle()
            .WithArguments(Container, canvasInstance, _uiPanelConfig).NonLazy();
        
        // После биндингов выполните инъекцию для Canvas и всех его детей,
        // чтобы компоненты (например, MenuPanel) получили корректно UIPanelService.
        Container.InjectGameObject(canvasInstance.gameObject);
    }
}