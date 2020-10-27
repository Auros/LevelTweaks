using Zenject;
using LevelTweaks.Configuration;

namespace LevelTweaks.Installers
{
    internal class LevelTweaksCoreInstaller : Installer<Config, LevelTweaksCoreInstaller>
    {
        private readonly Config _config;

        public LevelTweaksCoreInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
        }
    }
}