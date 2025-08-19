using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DoomLibrary.model
{
    class ModsManager
    {
        public static string selectedWad = ""; 
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

            Trace.WriteLine(JsonSerializer.Serialize(allMods));
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

        public static void SaveMods()
        {
            string serialized = JsonSerializer.Serialize(new ModsManagerSerializer(selectedWad, allMods, lastLoadOrder));
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\mods.json", serialized);
        }

        public static void LoadMods()
        {
            ModsManagerSerializer deserialized = new ModsManagerSerializer("", new List<Mod>(), 0);
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\mods.json"))
            {
                string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\mods.json");
                deserialized = JsonSerializer.Deserialize<ModsManagerSerializer>(json);
            }

            selectedWad = deserialized.selectedWad;
            allMods = deserialized.allMods;
            lastLoadOrder = deserialized.lastLoadOrder;
        }
    }

    class ModsManagerSerializer {
        public string selectedWad { get; set; }
        public List<Mod> allMods { get; set; }
        public int lastLoadOrder { get; set; }

        public ModsManagerSerializer() { }

        public ModsManagerSerializer(string selectedWad, List<Mod> allMods, int lastLoadOrder)
        {
            this.selectedWad = selectedWad;
            this.allMods = allMods;
            this.lastLoadOrder = lastLoadOrder;
        }
    }
}
