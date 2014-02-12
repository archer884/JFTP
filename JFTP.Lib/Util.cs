using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFTP.Lib
{
    public static class Config
    {
        public const int ChunkIncrement = 32768;
    }

    public static class ContentType
    {
        public const string File = "application/octet-stream";
    }

    public static class Util
    {
        #region Base36
        private const string AlfaBase36 = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string EncodeBase36(this long n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("XXX: Input must be greater than or equal to zero.");

            var result = new Stack<char>();
            while (n != 0)
            {
                result.Push(AlfaBase36[(int)(n % 36)]);
                n /= 36;
            }
            return new string(result.ToArray());
        }

        public static long DecodeBase36(this string n)
        {
            return n.Reverse()
                .Select((c, i) => new { CharValue = c, Index = i })
                .Sum(place => AlfaBase36.IndexOf(place.CharValue) * (long)Math.Pow(36, place.Index));
        }
        #endregion

        #region Storage
        public static long Kilobytes(this int n)
        {
            return n * 1024L;
        }

        public static long Megabytes(this int n)
        {
            return n * 1024L * 1024L;
        }
        #endregion

        public static TimeSpan TimeExecution(Action action)
        {
            var clock = new System.Diagnostics.Stopwatch();
            clock.Start();
            action();
            clock.Stop();
            return clock.Elapsed;
        }
    }
}
