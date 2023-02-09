using System.Windows.Media;

namespace QuizApp.BL
{
    public static class Util
    {
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child != null)
                {
                    break;
                }
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }

            }
            return child;
        }
    }
}
