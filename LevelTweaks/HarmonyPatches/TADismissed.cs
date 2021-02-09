using HarmonyLib;
using IPA.Loader;
using System;
using System.Reflection;

namespace LevelTweaks.HarmonyPatches
{
    [HarmonyPatch]
    internal class TADismissed
    {
        internal static event Action TARoomDismissed;

        private static MethodBase TargetMethod()
        {
            PluginMetadata tournamentAssistant = PluginManager.GetPluginFromId("TournamentAssistant");
            // Patch should only trigger if CameraPlus is installed and a pre-layerfix version
            if (tournamentAssistant != null && tournamentAssistant.Assembly.GetName().Version >= new Version("0.4.0"))
            {
                return tournamentAssistant.Assembly.GetType("TournamentAssistant.UI.FlowCoordinators.RoomCoordinator").GetMethod("Dismiss", BindingFlags.Instance | BindingFlags.Public);
            }
            else return null;
        }

        private static void Postfix()
        {
            TARoomDismissed?.Invoke();
        }
    }
}
