using System;
using SaintCoinach.Xiv;

namespace Shared.SqPack
{
    public static class Extensions
    {
        public static bool TryGetValue<T>(this IXivSheet<T> sheet, uint key, out T entry) where T : XivRow
        {
            try
            {
                entry = sheet[(int)key];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                entry = null;
                return false;
            }
        }
    }
}
