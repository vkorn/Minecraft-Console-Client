using System.Collections.Concurrent;
using System.Linq;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character
{
    public class Radar
    {
        public delegate bool MobSelector(MobTypes t);

        public ConcurrentDictionary<int, IMob> Mobs { get; private set; }

        public Radar(IMinecraftCom protocol, IMinecraftComHandler handler)
        {
            Mobs = new ConcurrentDictionary<int, IMob>();
        }

        internal void Spawn(IMob mob)
        {
            if (null == mob)
            {
                return;
            }

            if (Mobs.TryGetValue(mob.Id(), out _))
            {
                Mobs[mob.Id()] = mob;
                return;
            }

            Mobs.TryAdd(mob.Id(), mob);
        }

        internal void UpdatePosition(int id, Location pos, bool isRelative)
        {
            if (!Mobs.TryGetValue(id, out var mob))
            {
                return;
            }

            mob.UpdatePosition(pos, isRelative);
        }

        internal void Remove(int id)
        {
            Mobs.TryRemove(id, out _);
        }

        public IMob GetNearestMob(Location pos, MobSelector selector)
        {
            return Mobs.Where(x => selector(x.Value.Type()))
                .OrderBy(x => x.Value.Position().DistanceSquared(pos))
                .Select(x => x.Value).FirstOrDefault();
        }

        public IMob Get(int id)
        {
            return !Mobs.TryGetValue(id, out var mob) ? null : mob;
        }
    }
}