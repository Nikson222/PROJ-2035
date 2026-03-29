using System.ComponentModel;
using _Scripts.Game;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class LevelEditorInstaller : MonoInstaller
    {
        [SerializeField] private DotColorConfig _colorConfig;

        public override void InstallBindings()
        {
            Container.Bind<DotColorConfig>().FromInstance(_colorConfig).AsSingle();
        }
    }
}