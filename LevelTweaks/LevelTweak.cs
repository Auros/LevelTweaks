using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace LevelTweaks
{
    public class LevelTweak : CustomCellInfo
    {
        public float offset;
        public float njs;

        public LevelTweak() : base ("", "", null)
        {

        }

        public LevelTweak(LevelTweakInfo info) : base("", "", null)
        {
            offset = info.offset;
            njs = info.njs;
            text = info.name + $" <color=#878787><size=65%>Offset: {offset} | NJS: {njs}</size></color>";
        }
    }
}
