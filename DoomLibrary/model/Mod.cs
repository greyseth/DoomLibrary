using System;
using System.Collections.Generic;
using System.Text;

namespace DoomLibrary.model
{
    public class Mod
    {
        public string name { get; set; }
        public int loadOrder { get; set; }
        public bool hidden { get; set; }

        public Mod() { }

        public Mod(string name, int loadOrder, bool hidden)
        {
            this.name = name;
            this.loadOrder = loadOrder;
            this.hidden = hidden;
        }
    }
}
