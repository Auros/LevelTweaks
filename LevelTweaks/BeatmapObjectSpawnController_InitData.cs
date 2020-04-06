using HarmonyLib;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelTweaks
{
    [HarmonyPatch(typeof(BeatmapObjectSpawnController), "Start")]
    internal class BeatmapObjectSpawnController_InitData
    {
        internal static bool Prefix(ref BeatmapObjectSpawnController.InitData ____initData)
        {
            var data = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
            if (Configuration.Config.Instance.Tweaks.Any(x => x.LevelInfo.Equals(data.difficultyBeatmap, Plugin.lastSelectedMode) && x.Selected))
            {
                var tweak = Configuration.Config.Instance.Tweaks.Where(x => x.LevelInfo.Equals(data.difficultyBeatmap, Plugin.lastSelectedMode) && x.Selected).FirstOrDefault();
                
                Logger.log.Info($"offset: {tweak.Offset}, njs: {tweak.NJS}");

                if (tweak.NJS != data.difficultyBeatmap.noteJumpMovementSpeed)
                    BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("LevelTweaks");
                else
                    BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("LevelTweaks");

                ____initData.SetField("noteJumpMovementSpeed", tweak.NJS);
                ____initData.SetField("noteJumpStartBeatOffset", tweak.Offset);
            }

            return true;

        }
    }
}
