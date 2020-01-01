using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace LevelTweaks
{
    public class Plugin : IBeatSaberPlugin, IDisablablePlugin
    {
        internal static string Name => "LevelTweaks";

        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;
        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;
            config = configProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig()
                    {
                         
                    });
                }
                config = v;
            });
            Utilities.HarmonyUtil.InitHarmony("com.auros.leveltweaks");
        }

        public void OnEnable()
        {
            new GameObject().AddComponent<LevelTweaksManager>();
            Utilities.HarmonyUtil.Patch();
        }

        public void OnDisable()
        {
            
        }

        public void OnApplicationStart()
        {

        }

        public void OnApplicationQuit()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {

        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {



        }

        public void OnSceneUnloaded(Scene scene)
        {

        }
    }
}
