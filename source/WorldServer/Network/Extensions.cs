using System.Collections;

namespace WorldServer.Network
{
    public static class Extensions
    {
        public static byte[] ToArray(this BitArray bitArray)
        {
            byte[] buffer = new byte[bitArray.Length / 8];
            bitArray.CopyTo(buffer, 0);
            return buffer;
        }
    }
}
