using System.IO;
using System.Reflection;
using Ionic.Zip;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal abstract class RegistryProcessor : IRegistryProcessor
    {
        protected abstract ProtocolVersions MinVersion { get; }

        protected virtual string ResourceName => "registries.json.zip";

        ProtocolVersions IWorldProcessor.MinVersion()
        {
            return MinVersion;
        }

        public abstract IItem GetItem(int id);

        protected RegistryProcessor()
        {
            if (string.IsNullOrEmpty(ResourceName))
            {
                return;
            }

            using (var ms = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"MinecraftClient.Resources.{MinVersion.ToString()}.{ResourceName}"))
            {
                using (var zip = ZipFile.Read(ms))
                {
                    foreach (var f in zip)
                    {
                        using (var ds = new MemoryStream())
                        {
                            f.Extract(ds);
                            ProcessData(ds);
                        }

                        break;
                    }
                }
            }
        }

        protected abstract void ProcessData(MemoryStream ms);
    }
}