using IPA;
using HarmonyLib;
using SiraUtil.Zenject;
using System.Reflection;
using IPA.Config.Stores;
using LevelTweaks.Installers;
using Conf = IPA.Config.Config;
using LevelTweaks.Configuration;
using IPALogger = IPA.Logging.Logger;

namespace LevelTweaks
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Harmony harmony;

        internal static string lastSelectedMode = "Standard";
        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector)
        {
            Logger.Log = logger;
            zenjector.OnApp<LevelTweaksCoreInstaller>().WithParameters(conf.Generated<Config>());
            zenjector.OnMenu<LevelTweaksMenuInstaller>();
            harmony = new Harmony("dev.auros.leveltweaks");
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony?.UnpatchAll("dev.auros.leveltweaks");
        }
    }
}