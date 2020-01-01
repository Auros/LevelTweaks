using System.Collections.Generic;

namespace LevelTweaks
{
    internal class PluginConfig
    {
        public bool RegenerateConfig = true;
        public Dictionary<string, LevelTweakInfo> tweaks = new Dictionary<string, LevelTweakInfo>();
    }
}
