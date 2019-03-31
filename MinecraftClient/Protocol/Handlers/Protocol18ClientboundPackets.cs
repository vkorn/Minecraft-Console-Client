using System;
using System.Collections.Generic;

namespace MinecraftClient.Protocol.Handlers
{
    class KeepAlive
    {
        public static int getPacketID(int protocol)
        {
            if (protocol < PacketUtils.MC19Version)
                return 0x00;
            else if (protocol < PacketUtils.MC17w13aVersion)
                return 0x1F;
            else if (protocol < PacketUtils.MC112pre5Version)
                return 0x20;
            else if (protocol < PacketUtils.MC17w31aVersion)
                return 0x1F;
            else if (protocol < PacketUtils.MC18w01aVersion)
                return 0x20;
            else
                return 0x21;
        }
    }

    class JoinGame
    {
        public static int getPacketID(int protocol)
        {
            if (protocol < PacketUtils.MC19Version)
                return 0x01;
            else if (protocol < PacketUtils.MC17w13aVersion)
                return 0x23;
            else if (protocol < PacketUtils.MC112pre5Version)
                return 0x24;
            else if (protocol < PacketUtils.MC17w31aVersion)
                return 0x23;
            else if (protocol < PacketUtils.MC18w01aVersion)
                return 0x24;
            else
                return 0x25;
        }
    }
}
