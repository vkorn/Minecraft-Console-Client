using System.IO;
using System.Reflection;
using Ionic.Zip;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors
{
    internal abstract class MaterialLoader
    {
        protected abstract string ResourceName { get; }

        protected MaterialLoader()
        {
            using (var ms = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"MinecraftClient.Resources.{ResourceName}"))
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