using System.Collections.Generic;
using System.Linq;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.PluginMessage
{
    internal class PluginMessageOut18 : PluginMessageOut
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return PacketUtils.concatBytes(PacketUtils.getString(((PluginMessageRequest) data).Channel),
                packetData.ToArray());
        }
    }
}