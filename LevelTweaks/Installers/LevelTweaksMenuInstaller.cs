using System;
using Zenject;
using LevelTweaks.UI;

namespace LevelTweaks.Installers
{
    internal class LevelTweaksMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<LTUI>().AsSingle();
        }
    }
}