using System.IO;

namespace Compression
{
    public class LZ11
    {
        public static void Compress(Stream input, Stream output)
        {
            int sourceLength = (int)input.Length;
            byte[] sourceArray = new byte[sourceLength];
            input.Read(sourceArray, 0, sourceLength);
            int sourcePointer = 0;
            int destinationPointer = 4;
            LZWindow dictionary = new LZWindow(4096, 4096);
            if (sourceLength <= 16777215)
                WriteInt32(output, 0x11 | (sourceLength << 8));
            else
            {
                output.WriteByte(0x11);
                WriteInt32(output, sourceLength);
                destinationPointer += 4;
            }
            while (sourcePointer < sourceLength)
            {
                using MemoryStream buffer = new MemoryStream();
                byte flag = 0;
                for (int i = 7; i >= 0; i--)
                {
                    int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);
                    if (match[1] > 0)
                    {
                        flag = (byte)(flag | (byte)(1 << i));
                        if (match[1] < 0x11)
                        {
                            buffer.WriteByte((byte)((((match[1] - 1) & 0xF) << 4) | (((match[0] - 1) & 0xFFF) >> 8)));
                            buffer.WriteByte((byte)((uint)(match[0] - 1) & 0xFFu));
                        }
                        else if (match[1] < 0x111)
                        {
                            buffer.WriteByte((byte)(((match[1] - 0x11) & 0xFF) >> 4));
                            buffer.WriteByte((byte)((((match[1] - 0x11) & 0xF) << 4) | (((match[0] - 1) & 0xFFF) >> 8)));
                            buffer.WriteByte((byte)((uint)(match[0] - 1) & 0xFFu));
                        }
                        else
                        {
                            buffer.WriteByte((byte)(0x10u | (uint)(((match[1] - 0x111) & 0xFFFF) >> 12)));
                            buffer.WriteByte((byte)(((match[1] - 0x111) & 0xFFF) >> 4));
                            buffer.WriteByte((byte)((((match[1] - 0x111) & 0xF) << 4) | (((match[0] - 1) & 0xFFF) >> 8)));
                            buffer.WriteByte((byte)((uint)(match[0] - 1) & 0xFFu));
                        }
                        dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);
                        dictionary.SlideWindow(match[1]);
                        sourcePointer += match[1];
                    }
                    else
                    {
                        buffer.WriteByte(sourceArray[sourcePointer]);
                        dictionary.AddEntry(sourceArray, sourcePointer);
                        dictionary.SlideWindow(1);
                        sourcePointer++;
                    }
                    if (sourcePointer >= sourceLength)
                        break;
                }
                output.WriteByte(flag);
                buffer.Position = 0L;
                while (buffer.Position < buffer.Length)
                {
                    byte value = ReadByte(buffer);
                    output.WriteByte(value);
                }
                destinationPointer += (int)buffer.Length + 1;
            }
        }

        public static void Decompress(Stream input, Stream output)
        {
            byte[] data = new byte[input.Length];
            input.Read(data, 0, data.Length);
            uint decompLength = (uint)(data[1] | (data[2] << 8) | (data[3] << 16));
            byte[] result = new byte[decompLength];
            int offset = 4;
            int destOffset = 0;
            while (true)
            {
                byte header = data[offset++];
                for (int i = 0; i < 8; i++)
                {
                    if ((header & 0x80) == 0)
                        result[destOffset++] = data[offset++];
                    else
                    {
                        byte a, b, c, d;
                        int length, offs;
                        a = data[offset++];
                        b = data[offset++];
                        switch (a >> 4)
                        {
                            case 0:
                                {
                                    c = data[offset++];
                                    length = (((a & 0xF) << 4) | (b >> 4)) + 0x11;
                                    offs = (((b & 0xF) << 8) | c) + 1;
                                }
                                break;

                            case 1:
                                {
                                    c = data[offset++];
                                    d = data[offset++];
                                    length = (((a & 0xF) << 12) | (b << 4) | (c >> 4)) + 0x111;
                                    offs = (((c & 0xF) << 8) | d) + 1;
                                }
                                break;

                            default:
                                offs = (((a & 0xF) << 8) | b) + 1;
                                length = (a >> 4) + 1;
                                break;
                        }

                        for (int j = 0; j < length; j++)
                        {
                            result[destOffset] = result[destOffset - offs];
                            destOffset++;
                        }
                    }

                    if (destOffset >= decompLength)
                    {
                        output.Write(result, 0, result.Length);
                        return;
                    }

                    header = (byte)(header << 1);
                }
            }
        }

        private static byte ReadByte(Stream src)
        {
            int val = src.ReadByte();
            if (val == -1)
                throw new EndOfStreamException();

            return (byte)val;
        }

        private static void WriteInt32(Stream dest, int val)
        {
            dest.WriteByte((byte)((uint)val & 0xFFu));
            dest.WriteByte((byte)((uint)(val >> 8) & 0xFFu));
            dest.WriteByte((byte)((uint)(val >> 16) & 0xFFu));
            dest.WriteByte((byte)((uint)(val >> 24) & 0xFFu));
        }
    }

    internal class LZWindow
    {
        private int windowSize;

        private int windowStart;

        private int windowLength;

        private int minMatch;

        private int maxMatch;

        private List<int>[] offsets;

        internal LZWindow(int windowSize = 4096, int maxMatch = 18, int minMatch = 3)
        {
            this.windowSize = windowSize;
            this.maxMatch = maxMatch;
            this.minMatch = minMatch;
            offsets = new List<int>[256];
            for (int i = 0; i < offsets.Length; i++)
                offsets[i] = new List<int>();
        }

        internal int[] Search(byte[] data, uint offset, uint length)
        {
            RemoveOldEntries(data[offset]);
            if (offset < minMatch || length - offset < minMatch)
                return new int[2];

            int[] match = { 0, 0 };
            for (int i = offsets[data[offset]].Count - 1; i >= 0; i--)
            {
                int matchStart = offsets[data[offset]][i];
                int matchSize;
                for (matchSize = 1; matchSize < maxMatch && matchSize < windowLength && matchStart + matchSize < offset && offset + matchSize < length && data[offset + matchSize] == data[matchStart + matchSize]; matchSize++);

                if (matchSize >= minMatch && matchSize > match[1])
                {
                    match = new int[2] { (int)(offset - matchStart), matchSize };
                    if (matchSize == maxMatch)
                        break;
                }
            }
            return match;
        }

        internal void SlideWindow(int amount)
        {
            if (windowLength == windowSize)
            {
                windowStart += amount;
                return;
            }
            if (windowLength + amount <= windowSize)
            {
                windowLength += amount;
                return;
            }
            amount -= windowSize - windowLength;
            windowLength = windowSize;
            windowStart += amount;
        }

        private void RemoveOldEntries(byte index)
        {
            int i = 0;
            while (i < offsets[index].Count && offsets[index][i] < windowStart)
            {
                offsets[index].RemoveAt(0);
            }
        }

        internal void AddEntry(byte[] data, int offset)
        {
            offsets[data[offset]].Add(offset);
        }

        internal void AddEntryRange(byte[] data, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                AddEntry(data, offset + i);
            }
        }
    }

}
