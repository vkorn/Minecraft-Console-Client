using System;
using MinecraftClient.Mapping;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal class Mob : IMob
    {
        private MobEntry _entry;
        private Guid _uuid;
        private Location _pos;
        private int _id;

        public Mob(MobEntry entry, int id, Guid uuid, Location pos)
        {
            _entry = entry;
            _uuid = uuid;
            _pos = pos;
            _id = id;
        }

        public int Id()
        {
            return _id;
        }

        public Guid Uuid()
        {
            return _uuid;
        }

        public string Name()
        {
            return _entry.Name;
        }

        public Location Position()
        {
            return _pos;
        }

        public void UpdatePosition(Location delta, bool isRelative)
        {
            if (!isRelative)
            {
                _pos = delta;
                return;
            }

            _pos.X = (delta.X / 128 + _pos.X * 32) / 32;
            _pos.Y = (delta.Y / 128 + _pos.Y * 32) / 32;
            _pos.Z = (delta.Z / 128 + _pos.Z * 32) / 32;
        }

        public MobTypes Type()
        {
            return _entry.Type;
        }
    }
}