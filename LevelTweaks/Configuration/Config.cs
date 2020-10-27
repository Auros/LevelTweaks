using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace LevelTweaks.Configuration
{
    public class Config
    {
        [UseConverter(typeof(ListConverter<TweakData>))]
        [NonNullable]
        public virtual List<TweakData> Tweaks { get; set; } = new List<TweakData>();
    }
}