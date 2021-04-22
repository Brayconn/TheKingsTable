using System.Reflection;
using ScintillaNET;

namespace CaveStoryEditor
{
    public static class ScintillaExtensions
    {
        internal static FieldInfo StylingPositionField = typeof(Scintilla)
            .GetField("stylingPosition", BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);

        public static int GetStylingPosition(this Scintilla scintilla) => (int)StylingPositionField.GetValue(scintilla);

        public static void Style(this Scintilla scintilla, int length, int styling)
        {
            if (scintilla.GetStylingPosition() + length > scintilla.TextLength)
                length = scintilla.TextLength - scintilla.GetStylingPosition();
            if(length > 0)
                scintilla.SetStyling(length, styling);
        }
    }
}
