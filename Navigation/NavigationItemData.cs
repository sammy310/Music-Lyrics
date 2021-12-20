using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MusicLyrics.Navigation
{
    public class NavigationItemData
    {
        public NavigationItemData(string name, Type pageType, Symbol iconSymbol)
        {
            Name = name;
            PageType = pageType;
            IconSymbol = iconSymbol;
        }

        public string Name { get; private set; }
        public Type PageType { get; private set; }
        public Symbol IconSymbol { get; private set; }
        
        public override string ToString()
        {
            return this.Name;
        }
    }
}
