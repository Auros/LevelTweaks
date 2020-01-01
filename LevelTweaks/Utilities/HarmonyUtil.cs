using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LevelTweaks.Utilities
{
    public class HarmonyUtil
    {
        internal static HarmonyInstance harmony;

        public static void InitHarmony(string id) => harmony = HarmonyInstance.Create(id);

        public static void Patch() => harmony.PatchAll(Assembly.GetExecutingAssembly());


        public static void Unpatch() => harmony.UnpatchAll();
    }
}
