using LevelTweaks.Configuration;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace LevelTweaks
{
    public class TweakCell : CustomCellInfo
    {
        public TweakData data;
        public bool isDefault = false;
        public readonly float bpm; 

        public TweakCell() : base ("", "", null) { }

        public TweakCell(TweakData tweak, float bpm, bool def = false) : base("", "", null)
        {
            isDefault = def;
            this.bpm = bpm;
            data = tweak;
            text = $"{data.Name} <color=#a8a8a8><size=65%>NJS: {data.NJS} | Offset: {data.Offset} | Jump Distance: {CalculateJumpDistance():F2}</size></color>";
        }

        public void UpdateText()
        {
            text = $"{data.Name} <color=#a8a8a8><size=65%>NJS: {data.NJS} | Offset: {data.Offset} | Jump Distance: {CalculateJumpDistance():F2}</size></color>";
        }

        private float CalculateJumpDistance()
        {
            float num = 60f / bpm;
            float halfJump = 4f;

            while (data.NJS * num * halfJump > 18)
                halfJump /= 2;

            halfJump += data.Offset;

            if (halfJump < 1) halfJump = 1f;
            return data.NJS * num * halfJump * 2;
        }
    }
}