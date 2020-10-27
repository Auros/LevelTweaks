using Zenject;
using HarmonyLib;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Services;
using LevelTweaks.Configuration;

namespace LevelTweaks
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    internal class GameplayCoreInstaller_InstallBindings
    {
        internal static void Postfix(ref GameplayCoreInstaller __instance)
        {
            var mib = __instance as MonoInstallerBase;
            var Container = SiraUtil.Accessors.GetDiContainer(ref mib);
            bool isNotMultiplayer = !Container.HasBinding<MultiplayerLevelSceneSetupData>();
            if (isNotMultiplayer)
            {
                var config = Container.Resolve<Config>();
                var submit = Container.Resolve<Submission>();
                var setupData = Container.Resolve<GameplayCoreSceneSetupData>();
                var spawnControllerInitData = Container.Resolve<BeatmapObjectSpawnController.InitData>();
                if (config.Tweaks.Any(x => x.LevelInfo.Equals(setupData.difficultyBeatmap, Plugin.lastSelectedMode) && x.Selected))
                {
                    var tweak = config.Tweaks.Where(x => x.LevelInfo.Equals(setupData.difficultyBeatmap, Plugin.lastSelectedMode) && x.Selected).FirstOrDefault();

                    if (tweak.NJS != setupData.difficultyBeatmap.noteJumpMovementSpeed)
                    {
                        submit.DisableScoreSubmission("LevelTweaks", "Different NJS");
                    }
                    if (tweak.Offset != setupData.difficultyBeatmap.noteJumpStartBeatOffset)
                    {
                        submit.DisableScoreSubmission("LevelTweaks", "Different Offset");
                    }

                    spawnControllerInitData.SetField("noteJumpMovementSpeed", tweak.NJS);
                    spawnControllerInitData.SetField("noteJumpStartBeatOffset", tweak.Offset);
                }
            }
        }
    }
}