//using System;
//using System.Collections.Generic;
//
//namespace MinecraftClient.Protocol.Handlers
//{
//    /// <summary>
//    /// Implementation of the Serverbound Keep Alive Packet
//    /// https://wiki.vg/Protocol#Keep_Alive_.28serverbound.29
//    /// </summary>
//    class ServerKeepAlive
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x00;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x0B;
//            if (protocol < PacketUtils.MC112pre5Version)
//                return 0x0C;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x0B;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                return 0x0A;
//            if (protocol < PacketUtils.MC113pre4Version)
//                return 0x0B;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x0C;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x0E;
//            return 0x0F;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Resource Pack Status Packet
//    /// https://wiki.vg/Protocol#Resource_Pack_Status
//    /// </summary>
//    class ServerResourcePackStatus
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x19;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x16;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x18;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                return 0x17;
//            if (protocol < PacketUtils.MC113pre4Version)
//                return 0x18;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x1B;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x1D;
//            return 0x1F;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Chat Message Packet
//    /// https://wiki.vg/Protocol#Chat_Message_.28serverbound.29
//    /// </summary>
//    class ServerChatMessage
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x01;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x02;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x03;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x02;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x01;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x02;
//            return 0x03;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Client Status Packet
//    /// https://wiki.vg/Protocol#Client_Status
//    /// </summary>
//    class ServerClientStatus
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x16;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x03;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x04;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x03;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x02;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x03;
//            return 0x04;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Client Settings Packet
//    /// https://wiki.vg/Protocol#Client_Settings
//    /// </summary>
//    class ServerClientSettings
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x15;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x04;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x05;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x04;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x03;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x04;
//            return 0x05;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Plugin Message Packet
//    /// https://wiki.vg/Protocol#Plugin_Message_.28serverbound.29
//    /// </summary>
//    class ServerPluginMessage
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x17;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x09;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x0A;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x09;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                return 0x08;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x09;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x0A;
//            return 0x0B;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Tab-Complete Packet
//    /// https://wiki.vg/Protocol#Tab-Complete_.28serverbound.29
//    /// </summary>
//    class ServerTabComplete
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x14;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x01;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x02;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x01;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                // throw new InvalidOperationException("TabComplete was accidentely removed in protocol " + protocol + ". Please use a more recent version.");
//                return -1;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x04;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x05;
//            return 0x06;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Player Position Packet
//    /// https://wiki.vg/Protocol#Player_Position
//    /// </summary>
//    class ServerPlayerPosition
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x04;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x0C;
//            if (protocol < PacketUtils.MC112pre5Version)
//                return 0x0D;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x0E;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x0D;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                return 0x0C;
//            if (protocol < PacketUtils.MC113pre4Version)
//                return 0x0D;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x0E;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x10;
//            return 0x11;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Player Position And Look Packet
//    /// https://wiki.vg/Protocol#Player_Position_And_Look_.28serverbound.29
//    /// </summary>
//    class ServerPlayerPositionAndLook
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                return 0x06;
//            if (protocol < PacketUtils.MC17w13aVersion)
//                return 0x0D;
//            if (protocol < PacketUtils.MC112pre5Version)
//                return 0x0E;
//            if (protocol < PacketUtils.MC17w31aVersion)
//                return 0x0F;
//            if (protocol < PacketUtils.MC17w45aVersion)
//                return 0x0E;
//            if (protocol < PacketUtils.MC17w46aVersion)
//                return 0x0D;
//            if (protocol < PacketUtils.MC113pre4Version)
//                return 0x0E;
//            if (protocol < PacketUtils.MC113pre7Version)
//                return 0x0F;
//            if (protocol < PacketUtils.MC114pre5Version)
//                return 0x11;
//            return 0x12;
//        }
//    }
//
//    /// <summary>
//    /// Implementation of the Serverbound Teleport Confirm Packet
//    /// https://wiki.vg/Protocol#Teleport_Confirm
//    /// </summary>
//    class ServerTeleportConfirm
//    {
//        public static int getPacketID(int protocol)
//        {
//            if (protocol < PacketUtils.MC19Version)
//                // throw new InvalidOperationException("Teleport confirm is not supported in protocol " + protocol);
//                return -1;
//            else
//                return 0x00;
//        }
//    }
//}
