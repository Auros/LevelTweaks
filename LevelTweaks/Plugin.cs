using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace LevelTweaks
{
    public class Plugin : IBeatSaberPlugin
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
