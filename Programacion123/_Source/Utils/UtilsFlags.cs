using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class Flags
    {
        public static uint FromBit(int n) { return (1U << n); }
        public static uint Empty() { return 0; }
        public static void Clear(ref uint f) { f = 0; }
        public static bool IsEmpty(uint f) { return f == 0; }
        public static void Add(ref uint f1, uint f2) { f1 |= f2; }
        public static uint Add(uint f1, uint f2) { return f1 | f2; }
        public static uint Remove(uint f1, uint f2) { return f1 & ~f2; }
        public static void Remove(ref uint f1, uint f2) { f1 &= ~f2; }
        public static bool Test(uint f1, uint f2) { return (f1 & f2) != 0; }
    }

}
