using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.SPU.TGR.Shares
{
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
    }
}
