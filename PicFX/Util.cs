using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PicFX
{
    class Util
    {
        public static string PutTogether(string[] pieces)
        {
            string finalString = "";
            string preString = "";
            for (int i = 1; i < pieces.Length; i++)
                preString += pieces[i] + " ";
            for (int i = 0; i < preString.Length; i++)
                if (preString[i] == '/') finalString += '\\';
                else finalString += preString[i];
            return finalString.Remove(finalString.Length - 1);
        }
    }
}
