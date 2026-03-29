using _Scripts._Infrastructure.Constants;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class PopupInstaller : MonoInstaller
    {
        [SerializeField] private PopupText _popupTextPrefab;

        public override void InstallBindings()
        {
            var canvasRectTransform = CreateCanvas();

            Container.Bind<RectTransform>().WithId(NameConstants.PopupCanvasName).FromInstance(canvasRectTransform).AsSingle();

            Container.BindMemoryPool<PopupText, PopupText.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_popupTextPrefab)
                .UnderTransform(canvasRectTransform);

            Container.BindInterfacesAndSelfTo<PopupTextService>().AsSingle().NonLazy();
        }

        private RectTransform CreateCanvas()
        {
            var canvasGameObject = new GameObject(NameConstants.PopupCanvasName);

            var canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = canvasGameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

            if (canvasGameObject.GetComponent<RectTransform>() == null)
            {
                canvasGameObject.AddComponent<RectTransform>();
            }

            DontDestroyOnLoad(canvasGameObject);

            return canvasGameObject.GetComponent<RectTransform>();
        }
    }
}