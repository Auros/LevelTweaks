using HarmonyLib;
using IPA.Loader;
using System;
using System.Reflection;

namespace LevelTweaks.HarmonyPatches
{
    [HarmonyPatch]
    internal class TADifficultySelected
    {
        internal static event Action<IDifficultyBeatmap> TADifficultyBeatmapSelected;

        private static MethodBase TargetMethod()
        {
            PluginMetadata tournamentAssistant = PluginManager.GetPluginFromId("TournamentAssistant");
            // Patch should only trigger if CameraPlus is installed and a pre-layerfix version
            if (tournamentAssistant != null && tournamentAssistant.Assembly.GetName().Version >= new Version("0.4.0"))
            {
                return tournamentAssistant.Assembly.GetType("TournamentAssistant.UI.FlowCoordinators.RoomCoordinator").GetMethod("songDetail_didChangeDifficultyBeatmapEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else return null;
        }

        private static void Postfix(ref IDifficultyBeatmap beatmap)
        {
            if (beatmap.parentDifficultyBeatmapSet != null)
            {
                TADifficultyBeatmapSelected?.Invoke(beatmap);
            }
        }
    }
}
