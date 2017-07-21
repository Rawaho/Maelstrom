using System.Data;

namespace Shared.Database.Authentication
{
    public struct VersionInfo
    {
        public string File { get; }
        public uint Version { get; }
        public string Digest { get; }

        public VersionInfo(DataRow row)
        {
            File    = row.Read<string>("file");
            Version = row.Read<uint>("version");
            Digest  = row.Read<string>("digest");
        }
    }
}
