using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DoomLibrary.model
{
    public class Mod : INotifyPropertyChanged
    {
        public string name { get; set; }

        private int loadOrder;
        public int LoadOrder{ get => loadOrder; set
            {
                if (loadOrder != value)
                {
                    loadOrder = value;
                    OnPropertyChanged(nameof(loadOrder));
                    OnPropertyChanged(nameof(IsApplied));
                }
            }
        }

        private bool hidden;
        public bool Hidden { get => hidden; set { 
            if (hidden != value)
                {
                    hidden = value;
                    OnPropertyChanged(nameof(Hidden));
                    OnPropertyChanged(nameof(HiddenText));
                }
            } 
        }

        public Mod() { }

        public Mod(string name, int loadOrder, bool hidden)
        {
            this.name = name;
            this.loadOrder = loadOrder;
            this.hidden = hidden;
        }

        public bool IsApplied
        {
            get { return loadOrder > 0; }
        }

        public string HiddenText
        {
            get { return hidden ? "Unhide Mod" : "Hide Mod"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
