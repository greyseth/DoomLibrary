using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoomLibrary.model
{
    class ModsManager
    {
        public static Dictionary<string, int> appliedMods = new Dictionary<string, int>();
        public static List<string> hiddenMods = new List<string>();
        public static bool showHidden = false;

        public static void ApplyMod(string mod)
        {
            appliedMods.Add(mod, appliedMods.Count + 1);
        }

        public static void RemoveMod(string mod)
        {
            appliedMods.Remove(mod);

            string[] appliedModsKeys = appliedMods.Keys.ToArray();
            int i = 1;
            foreach (string key in appliedModsKeys)
            {
                appliedMods[key] = i;
                i++;

            }
        }

        public static int GetLoadOrder(string mod)
        {
            return appliedMods.ContainsKey(mod) ? appliedMods[mod] : -1;
        }

        public static bool IsHidden(string mod)
        {
            if (hiddenMods.FindIndex(m => m == mod) != -1) return true;
            else return false;
        }
    }
}
