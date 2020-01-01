using Harmony;

namespace LevelTweaks.Harmony
{
    [HarmonyPatch(typeof(StandardLevelDetailViewController), "RefreshAvailabilityAsync")]
    public class StandardLevelDetailViewController_RefreshAvailabilityAsync
    {
        static void Prefix(ref StandardLevelDetailView ____standardLevelDetailView)
        {
            //LevelTweaksManager.Instance.CurrentlySelectedDifficultyBeatmap = ____standardLevelDetailView.selectedDifficultyBeatmap;
        }
    }
}
