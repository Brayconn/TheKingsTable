using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace CaveStoryEditor
{
    //Trying to work around the same issue as here:
    //https://github.com/UnknownShadow200/MCGalaxy/commit/2d87519af26089b770ece6b19d9eb475858a9ea0
    public sealed class PropertyGridShell : PropertyGrid
    {
        sealed class PropertiesTabShell : PropertiesTab
        {
            public override Bitmap Bitmap => base.Bitmap ?? new Bitmap(16,16);
        }
        protected override PropertyTab CreatePropertyTab(Type tabType) => new PropertiesTabShell();
    }
}
