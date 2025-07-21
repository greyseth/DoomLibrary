using System;
using System.Collections.Generic;
using System.Text;

namespace DoomLibrary.model
{
    public class SourcePort
    {
        public static int lastIndex;
        
        public string Path { get; set; }
        public int Index { get; set; }

        public SourcePort() { }

        public SourcePort(string path)
        {
            this.Path = path;
            this.Index = lastIndex;
            lastIndex++;
        }

        public string Name
        {
            get { string[] pathSplit = Path.Split("\\"); return pathSplit[^1]; }
        }
    }
}
