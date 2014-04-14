using System.Reflection;
using System.Windows.Controls;

namespace PsHandler
{
    public static class ExtensionMethods
    {
        public static void SetProperty(this object o, string propertyName, object property, bool setForChildren = false)
        {
            if (o == null) return;

            PropertyInfo pi = o.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                pi.SetValue(o, property, null);

                if (setForChildren)
                {
                    if (o is GroupBox)
                    {
                        (o as GroupBox).Content.SetProperty(propertyName, property, true);
                    }
                    if (o is Grid)
                    {
                        foreach (var child in (o as Grid).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is StackPanel)
                    {
                        foreach (var child in (o as StackPanel).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is UserControl)
                    {
                        (o as UserControl).Content.SetProperty(propertyName, property, true);
                    }
                }
            }
        }
    }
}
