using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoomLibrary.model
{
    class ModsManager
    {
        public static List<Mod> allMods = new List<Mod>();
        public static int lastLoadOrder = 0;

        public static void ApplyMod(string mod)
        {
            int modIndex = allMods.FindIndex(m => m.name == mod);
            if (modIndex != -1) allMods[modIndex].LoadOrder = lastLoadOrder + 1;
            lastLoadOrder++;
        }

        public static void RemoveMod(string mod)
        {
            int modIndex = allMods.FindIndex(m => m.name == mod);
            int prevLoadOrder = allMods[modIndex].LoadOrder;
            if (modIndex != -1) allMods[modIndex].LoadOrder = 0;

            int biggestModifiedLoadOrder = 0;
            foreach (Mod aMod in allMods)
            {
                if (aMod.LoadOrder > prevLoadOrder)
                {
                    aMod.LoadOrder -= 1;
                    if (aMod.LoadOrder > biggestModifiedLoadOrder) biggestModifiedLoadOrder = aMod.LoadOrder;
                }
            }

            lastLoadOrder = biggestModifiedLoadOrder;
        }

        public static int GetLoadOrder(string mod)
        {
            int modIndex = allMods.FindIndex(m => m.name == mod);
            if (modIndex != -1) return allMods[modIndex].LoadOrder;
            else return 0;
        }

        public static bool IsHidden(string mod)
        {
            int modIndex = allMods.FindIndex(m => m.name == mod);
            if (modIndex != -1) return allMods[modIndex].Hidden;
            else return false;
        }
    }
}
