using System;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.ChatBots
{
    public class MobFarm : ChatBot
    {
        public override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        
        private long _lastAttack;

        public MobFarm()
        {
            if (Settings.AntiHunger_Enabled)
            {
                ConsoleIO.WriteLineFormatted("Warning, potentially conflicting bots: MobFarm and AntiHunger");
            }
        }

        public override void Update()
        {
            if (false == GetPlayer().IsLoaded)
            {
                return;
            }
            
            var now = DateTime.Now.Ticks;

            if (now - _lastAttack < 1 * TimeSpan.TicksPerSecond)
            {
                // Too early
                return;
            }

            // We don't want to re-calculate too often
            _lastAttack = now;
            
            var mob = GetPlayer().Radar.GetNearestMob(GetCurrentLocation(), t => t == MobTypes.Mob);
            if (null == mob || mob.Position().DistanceSquared(GetCurrentLocation()) > 7) // around 2.6 blocks is safe
            {
                return;
            }
            
            GetPlayer().LookAt(mob.Position());
            GetPlayer().Attack(mob);
        }
    }
}