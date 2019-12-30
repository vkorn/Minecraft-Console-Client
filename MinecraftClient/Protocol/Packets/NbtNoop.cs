using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets
{
    /// <summary>
    /// Skipping NBT records in packets.
    /// Format description: https://wiki.vg/NBT
    /// </summary>
    internal class NbtNoop
    {
        private readonly List<byte> _data;
        public List<byte> SkippedData { get; }

        public NbtNoop(List<byte> packetData)
        {
            _data = packetData;
            SkippedData = new List<byte>();
        }

        public void Reset()
        {
            SkippedData.Clear();
        }

        public void SkipTag()
        {
            if (null == _data || 0 == _data.Count)
            {
                return;
            }

            var tag = PacketUtils.readNextByte(_data);
            SkippedData.Add(tag);
            SkipTag(tag, false);
        }

        private void SkipTag(byte tag, bool nameless)
        {
            if (tag != 0)
            {
               
            }
            
            switch (tag)
            {
                case 0: // TAG_End
                    return;
                case 1: // TAG_Byte
                    SkipByte(nameless);
                    return;
                case 2: // TAG_Short
                    SkipShort(nameless);
                    return;
                case 3: // TAG_Int
                    SkipInt(nameless);
                    return;
                case 4: // TAG_Long
                    SkipLong(nameless);
                    return;
                case 5: // TAG_Float
                    SkipFloat(nameless);
                    return;
                case 6: // TAG_Double
                    SkipDouble(nameless);
                    return;
                case 7: // TAG_Byte_Array 
                    SkipByteArray(nameless);
                    return;
                case 8: // TAG_String
                    SkipString(nameless);
                    return;
                case 9: // TAG_List
                    SkipList(nameless);
                    return;
                case 10: // TAG_Compound
                    SkipCompound(nameless);
                    return;
                case 11: // TAG_Int_Array
                    SkipIntArray(nameless);
                    return;
                case 12: // TAG_Long_Array
                    SkipLongArray(nameless);
                    return;
            }
        }

        private void SkipName(bool nameless)
        {
            if (nameless)
            {
                return;
            }

            var len = PacketUtils.readNextShort(_data);
            SkippedData.AddRange(PacketUtils.getShort(len));
            if (0 == len)
            {
                return;
            }

            SkippedData.AddRange(PacketUtils.readData(len, _data));
        }

        private void SkipByte(bool nameless)
        {
            SkipName(nameless);
            SkippedData.Add(PacketUtils.readNextByte(_data));
        }

        private void SkipShort(bool nameless)
        {
            SkipName(nameless);
            SkippedData.AddRange(PacketUtils.readData(2, _data));
        }

        private void SkipInt(bool nameless)
        {
            SkipName(nameless);
            SkippedData.AddRange(PacketUtils.readData(4, _data));
        }

        private void SkipLong(bool nameless)
        {
            SkipName(nameless);
            SkippedData.AddRange(PacketUtils.readData(8, _data));
        }

        private void SkipFloat(bool nameless)
        {
            SkipName(nameless);
            SkippedData.AddRange(PacketUtils.readData(4, _data));
        }

        private void SkipDouble(bool nameless)
        {
            SkipName(nameless);
            SkippedData.AddRange(PacketUtils.readData(8, _data));
        }

        private void SkipByteArray(bool nameless)
        {
            SkipName(nameless);
            var len = PacketUtils.readNextInt(_data);
            SkippedData.AddRange(PacketUtils.getInt(len));
            SkippedData.AddRange(PacketUtils.readData(len, _data));
        }

        private void SkipString(bool nameless)
        {
            SkipName(nameless);
            var len = PacketUtils.readNextShort(_data);
            SkippedData.AddRange(PacketUtils.getShort(len));
            SkippedData.AddRange(PacketUtils.readData(len, _data));
        }

        private void SkipIntArray(bool nameless)
        {
            SkipName(nameless);
            var len = PacketUtils.readNextInt(_data);
            SkippedData.AddRange(PacketUtils.getInt(len));
            SkippedData.AddRange(PacketUtils.readData(4 * len, _data));
        }

        private void SkipLongArray(bool nameless)
        {
            SkipName(nameless);
            var len = PacketUtils.readNextInt(_data);
            SkippedData.AddRange(PacketUtils.getInt(len));
            SkippedData.AddRange(PacketUtils.readData(8 * len, _data));
        }

        private void SkipList(bool nameless)
        {
            SkipName(nameless);
            var tag = PacketUtils.readNextByte(_data);
            SkippedData.Add(tag);
            var len = PacketUtils.readNextInt(_data);
            SkippedData.AddRange(PacketUtils.getInt(len));

            if (len <= 0)
            {
                // We don't care about type in this case.
                SkippedData.Add(PacketUtils.readNextByte(_data));
                return;
            }

            for (var ii = 0; ii < len; ii++)
            {
                SkipTag(tag, true);
            }
        }

        private void SkipCompound(bool nameless = false)
        {
            SkipName(nameless);

            var tag = PacketUtils.readNextByte(_data);
            SkippedData.Add(tag);
            while (tag != 0)
            {
                SkipTag(tag, false);
                tag = PacketUtils.readNextByte(_data);
                SkippedData.Add(tag);
            }
        }
    }
}