using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PsHandler
{
    public class Diff
    {
        public static List<string> FindNewLines(string textA, string textB)
        {
            List<string> a = textA.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> b = textB.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<string> bMatchA = new List<string>();

            foreach (var lb in b)
            {
                foreach (var la in a)
                {
                    if (lb.Equals(la))
                    {
                        bMatchA.Add(lb);
                        a.Remove(lb);
                        break;
                    }
                }
            }
            foreach (var lm in bMatchA)
            {
                b.Remove(lm);
            }

            return b;

            //
            if (false)
            {
                Debug.WriteLine("----------------------------------------------");
                foreach (var la in a)
                {
                    Debug.WriteLine("[deleted] " + la);
                }
                foreach (var lb in b)
                {
                    Debug.WriteLine("[added] " + lb);
                }
                Debug.WriteLine("----------------------------------------------");
            }
        }
    }
}
