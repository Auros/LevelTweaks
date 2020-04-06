using System.Runtime.CompilerServices;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace LevelTweaks.Configuration
{
    internal class Config
    {
        public static Config Instance { get; set; }

        [UseConverter(typeof(ListConverter<TweakData>))]
        [NonNullable]
        public virtual List<TweakData> Tweaks { get; set; } = new List<TweakData>();

        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }


        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }
    }
}