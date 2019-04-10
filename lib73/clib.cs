using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib73
{
    static class clib
    {
        public static string utf8(byte[] b) => Encoding.UTF8.GetString(b);
        public static byte[] utf8(string s) => Encoding.UTF8.GetBytes(s);
        public static int utf8c(string s) => Encoding.UTF8.GetByteCount(s);
        public static int utf8c(byte[] b) => Encoding.UTF8.GetCharCount(b);
    }
}
