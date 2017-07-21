/*
    xxHashSharp - A pure C# implementation of xxhash
    Copyright (C) 2014, Seok-Ju, Yun. (https://github.com/noricube/xxHashSharp)
    Original C Implementation Copyright (C) 2012-2014, Yann Collet. (https://code.google.com/p/xxhash/)
    BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

        * Redistributions of source code must retain the above copyright
          notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above
          copyright notice, this list of conditions and the following
          disclaimer in the documentation and/or other materials provided
          with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;

namespace Shared.Cryptography
{
    public class XxHash
    {
        public struct XxhState
        {
            public ulong TotalLen;
            public uint Seed;
            public uint V1;
            public uint V2;
            public uint V3;
            public uint V4;
            public int MemSize;
            public byte[] Memory;
        };

        protected XxhState state;

        private const uint Prime1 = 2654435761u;
        private const uint Prime2 = 2246822519u;
        private const uint Prime3 = 3266489917u;
        private const uint Prime4 = 668265263u;
        private const uint Prime5 = 374761393u;

        public XxHash()
        {
        }

        public static uint CalculateHash(byte[] buf, int len = -1, uint seed = 0)
        {
            uint h32;
            int index = 0;
            if (len == -1)
                len = buf.Length;

            if (len >= 16)
            {
                int limit = len - 16;
                uint v1 = seed + Prime1 + Prime2;
                uint v2 = seed + Prime2;
                uint v3 = seed + 0;
                uint v4 = seed - Prime1;

                do
                {
                    v1 = CalcSubHash(v1, buf, index);
                    index += 4;
                    v2 = CalcSubHash(v2, buf, index);
                    index += 4;
                    v3 = CalcSubHash(v3, buf, index);
                    index += 4;
                    v4 = CalcSubHash(v4, buf, index);
                    index += 4;
                } while (index <= limit);

                h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
            }
            else
                h32 = seed + Prime5;

            h32 += (uint)len;

            while (index <= len - 4)
            {
                h32 += BitConverter.ToUInt32(buf, index) * Prime3;
                h32 = RotateLeft(h32, 17) * Prime4;
                index += 4;
            }

            while (index < len)
            {
                h32 += buf[index] * Prime5;
                h32 = RotateLeft(h32, 11) * Prime1;
                index++;
            }

            h32 ^= h32 >> 15;
            h32 *= Prime2;
            h32 ^= h32 >> 13;
            h32 *= Prime3;
            h32 ^= h32 >> 16;

            return h32;
        }

        public void Init(uint seed = 0)
        {
            state.Seed = seed;
            state.V1 = seed + Prime1 + Prime2;
            state.V2 = seed + Prime2;
            state.V3 = seed + 0;
            state.V4 = seed - Prime1;
            state.TotalLen = 0;
            state.MemSize = 0;
            state.Memory = new byte[16];
        }

        public bool Update(byte[] input, int len)
        {
            int index = 0;

            state.TotalLen += (uint)len;

            if (state.MemSize + len < 16)
            {
                Array.Copy(input, 0, state.Memory, state.MemSize, len);
                state.MemSize += len;
                return true;
            }

            if (state.MemSize > 0)
            {
                Array.Copy(input, 0, state.Memory, state.MemSize, 16 - state.MemSize);

                state.V1 = CalcSubHash(state.V1, state.Memory, index);
                index += 4;
                state.V2 = CalcSubHash(state.V2, state.Memory, index);
                index += 4;
                state.V3 = CalcSubHash(state.V3, state.Memory, index);
                index += 4;
                state.V4 = CalcSubHash(state.V4, state.Memory, index);
                index += 4;

                index = 0;
                state.MemSize = 0;
            }

            if (index <= len - 16)
            {
                int limit = len - 16;
                uint v1 = state.V1;
                uint v2 = state.V2;
                uint v3 = state.V3;
                uint v4 = state.V4;

                do
                {
                    v1 = CalcSubHash(v1, input, index);
                    index += 4;
                    v2 = CalcSubHash(v2, input, index);
                    index += 4;
                    v3 = CalcSubHash(v3, input, index);
                    index += 4;
                    v4 = CalcSubHash(v4, input, index);
                    index += 4;
                } while (index <= limit);

                state.V1 = v1;
                state.V2 = v2;
                state.V3 = v3;
                state.V4 = v4;
            }

            if (index < len)
            {
                Array.Copy(input, index, state.Memory, 0, len - index);
                state.MemSize = len - index;
            }

            return true;
        }

        public uint Digest()
        {
            uint h32;
            if (state.TotalLen >= 16)
                h32 = RotateLeft(state.V1, 1) + RotateLeft(state.V2, 7) + RotateLeft(state.V3, 12) + RotateLeft(state.V4, 18);
            else
                h32 = state.Seed + Prime5;

            h32 += (uint)state.TotalLen;

            int index = 0;
            while (index <= state.MemSize - 4)
            {
                h32 += BitConverter.ToUInt32(state.Memory, index) * Prime3;
                h32 = RotateLeft(h32, 17) * Prime4;
                index += 4;
            }

            while (index < state.MemSize)
            {
                h32 += state.Memory[index] * Prime5;
                h32 = RotateLeft(h32, 11) * Prime1;
                index++;
            }

            h32 ^= h32 >> 15;
            h32 *= Prime2;
            h32 ^= h32 >> 13;
            h32 *= Prime3;
            h32 ^= h32 >> 16;

            return h32;
        }
        private static uint CalcSubHash(uint value, byte[] buf, int index)
        {
            uint readValue = BitConverter.ToUInt32(buf, index);
            value += readValue * Prime2;
            value = RotateLeft(value, 13);
            value *= Prime1;
            return value;
        }

        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}
