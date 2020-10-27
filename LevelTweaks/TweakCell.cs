using LevelTweaks.Configuration;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace LevelTweaks
{
    public class TweakCell : CustomCellInfo
    {
        public TweakData data;
        public bool isDefault = false;

        public TweakCell() : base ("", "", null) { }

        public TweakCell(TweakData tweak, bool def = false) : base("", "", null)
        {
            isDefault = def;
            data = tweak;
            text = $"{data.Name} <color=#a8a8a8><size=65%>NJS: {data.NJS} | Offset: {data.Offset}</size></color>";
        }

        public void UpdateText()
        {
            text = $"{data.Name} <color=#a8a8a8><size=65%>NJS: {data.NJS} | Offset: {data.Offset}</size></color>";
        }
    }
}