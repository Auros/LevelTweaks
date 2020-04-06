using BeatSaberMarkupLanguage.GameplaySetup;
using IPALogger = IPA.Logging.Logger;
using UnityEngine.SceneManagement;
using IPA.Config.Stores;
using System.Linq;
using IPA.Config;
using IPA;
using UnityEngine;
using HarmonyLib;

namespace LevelTweaks
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static string Name => "LevelTweaks";
        internal static Harmony harmony;

        internal static LevelFilteringNavigationController levelFilteringNavigationController;
        internal static string lastSelectedMode = "Standard";
        [Init]
        public Plugin(IPALogger logger, Config conf)
        {
            Instance = this;
            Logger.log = logger;
            Configuration.Config.Instance = conf.Generated<Configuration.Config>();

        }

        [OnEnable]
        public void OnEnable()
        {
            GameplaySetup.instance.AddTab("Level Tweaks", "LevelTweaks.UI.lt.bsml", UI.LTUI.instance);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;

            harmony = new Harmony($"com.auros.BeatSaber.{Name}");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony?.UnpatchAll();

            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "MenuViewControllers" && !levelFilteringNavigationController)
                levelFilteringNavigationController = Resources.FindObjectsOfTypeAll<LevelFilteringNavigationController>().FirstOrDefault();
            if (nextScene.name == "GameCore")
            {
                /*var data = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
                if (Configuration.Config.Instance.Tweaks.Any(x => x.LevelInfo.Equals(data.difficultyBeatmap, lastSelectedMode) && x.Selected))
                {
                    var tweak = Configuration.Config.Instance.Tweaks.Where(x => x.LevelInfo.Equals(data.difficultyBeatmap, lastSelectedMode) && x.Selected).FirstOrDefault();
                    var go = new GameObject("Level Tweaker").AddComponent<LevelTweaker>();
                    go.Load(tweak);
                    Logger.log.Info($"offset: {tweak.Offset}, njs: {tweak.NJS}");

                    if (tweak.NJS != data.difficultyBeatmap.noteJumpMovementSpeed)
                        BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("LevelTweaks");
                    else
                        BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("LevelTweaks");
                }*/

            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            
        }
    }
}
