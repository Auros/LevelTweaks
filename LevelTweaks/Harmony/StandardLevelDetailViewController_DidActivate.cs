using Harmony;

namespace LevelTweaks.Harmony
{
    [HarmonyPatch(typeof(StandardLevelDetailViewController), "DidActivate")]
    public class StandardLevelDetailViewController_DidActivate
    {
        static void Postfix(bool firstActivation, ref StandardLevelDetailView ____standardLevelDetailView)
        {
            if (firstActivation)
                LevelTweaksManager.Instance.SetupButton(____standardLevelDetailView);
            //LevelTweaksManager.Instance.CurrentlySelectedDifficultyBeatmap = ____standardLevelDetailView.selectedDifficultyBeatmap;
        }
    }
}
