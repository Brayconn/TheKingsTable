using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingsTable
{
    public static class Utilities
    {
        public static Rect PointsToRect(int x1, int y1, int x2, int y2, int addToBottomRight = 0)
        {
            var left = Math.Min(x1, x2);
            var top = Math.Min(y1, y2);
            var right = Math.Max(x2, x1) + addToBottomRight;
            var bottom = Math.Max(y2, y1) + addToBottomRight;
            return new Rect(new Point(left, top), new Point(right, bottom));
        }
    }
}
