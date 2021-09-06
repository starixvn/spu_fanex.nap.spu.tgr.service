namespace Fanex.NAP.SPU.TGR.Shares
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Utilities
    {
        public static IEnumerable<IEnumerable<T>> ChunkList<T>(this IEnumerable<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("ChunkSize must be greater than 0.");
            }

            while (list.Any())
            {
                yield return list.Take(chunkSize);
                list = list.Skip(chunkSize);
            }
        }

        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
