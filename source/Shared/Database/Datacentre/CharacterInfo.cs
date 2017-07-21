using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Cryptography;
using Shared.Game;
using Shared.SqPack;
using Shared.SqPack.GameTable;

namespace Shared.Database.Datacentre
{
    public class CharacterInfoJson
    {
        [JsonProperty(PropertyName = "content")]
        [JsonConverter(typeof(StringConverter))]
        public JArray Content { get; set; }
        [JsonProperty(PropertyName = "classname")]
        public string ClassName { get; set; }
        [JsonProperty(PropertyName = "classid")]
        public uint ClassId { get; set; }
    }

    public class CharacterInfo
    {
        public uint AccountId { get; }
        public ulong Id { get; private set; }
        public uint ActorId { get; private set; }
        public ushort RealmId { get; }
        public string Name { get; }
        
        public CharacterAppearanceInfo Appearance { get; }
        public WorldPosition SpawnPosition { get; private set; }
        
        public ReadOnlyCollection<CharacterClassInfo> Classes => new ReadOnlyCollection<CharacterClassInfo>(classes);

        public byte Voice
        {
            get => data[0];
            private set => data[0] = value;
        }

        public byte Guardian
        {
            get => data[1];
            private set => data[1] = value;
        }

        public byte BirthMonth
        {
            get => data[2];
            private set => data[2] = value;
        }

        public byte BirthDay
        {
            get => data[3];
            private set => data[3] = value;
        }

        public byte ClassJobId
        {
            get => data[4];
            private set => data[4] = value;
        }

        public byte ClassId { get; }
        
        private readonly byte[] data;
        private readonly CharacterClassInfo[] classes = new CharacterClassInfo[CharacterClassInfo.MaxClassId];

        public CharacterInfo(uint serviceAccountId, ushort realmId, string name, string json)
        {
            AccountId  = serviceAccountId;
            RealmId    = realmId;
            Name       = name;

            var characterInfo = JsonProvider.DeserialiseObject<CharacterInfoJson>(json);
            Appearance = new CharacterAppearanceInfo(characterInfo.Content.Value<JArray>(0));
            data       = characterInfo.Content.Skip(1).Values<byte>().ToArray();
        }

        public CharacterInfo(DataRow row)
        {
            data          = new byte[6];
            AccountId     = row.Read<uint>("serviceAccountId");
            Id            = row.Read<ulong>("id");
            ActorId       = row.Read<uint>("actorId");
            RealmId       = row.Read<ushort>("realmId");
            Name          = row.Read<string>("name");
            BirthMonth    = row.Read<byte>("birthMonth");
            BirthDay      = row.Read<byte>("birthDay");
            Guardian      = row.Read<byte>("guardian");
            Voice         = row.Read<byte>("voice");
            ClassJobId    = row.Read<byte>("classJobId");
            Appearance    = new CharacterAppearanceInfo(row);
            SpawnPosition = new WorldPosition(row);

            if (GameTableManager.ClassJobs.TryGetValue(ClassJobId, ExdLanguage.En, out ClassJobEntry entry))
                ClassId = (byte)entry.ClassId;
        }

        public void Finalise(ulong id, WorldPosition position)
        {
            Id            = id;
            ActorId       = XxHash.CalculateHash(Encoding.UTF8.GetBytes($"{id:X8}:{Name}"));
            SpawnPosition = position;
        }

        public void AddClassInfo(byte classId)
        {
            var classInfo = new CharacterClassInfo(classId);
            classes[classInfo.Id] = classInfo;
        }

        public void AddClassInfo(DataRow row)
        {
            var classInfo = new CharacterClassInfo(row);
            classes[classInfo.Id] = classInfo;
        }

        public bool Verify()
        {
            // TODO: verify remaining data
            if (!VerifyName(Name))
                return false;

            if (!Appearance.Verify())
                return false;

            return true;
        }

        public static bool VerifyName(string name)
        {
            string[] nameExplode = name.Split(' ');
            if (nameExplode.Length != 2)
                return false;

            // forename and surname together can't total more than 20 characters
            if (nameExplode[0].Length + nameExplode[1].Length > 20)
                return false;

            bool IsAcceptedSymbol(char c) => c == '-' || c == '\'';
            for (int i = 0; i < 2; i++)
            {
                // forename and surname separately must be between 2 and 15 characters
                if (nameExplode[i].Length < 2 || nameExplode[i].Length > 15)
                    return false;

                for (int j = 0; j < nameExplode[i].Length; j++)
                {
                    char c = nameExplode[i][j];
                    if (!char.IsLower(c) && !char.IsUpper(c) && !IsAcceptedSymbol(c))
                        return false;

                    // only the first character should be uppercase
                    if (char.IsUpper(c))
                    {
                        if (j == 0)
                            continue;
                        return false;
                    }

                    if (IsAcceptedSymbol(c))
                    {
                        char pc = nameExplode[i][j - 1];
                        // hyphens can't be used in succession or placed immediatley before or after apostrophes
                        if (c == '-' && (pc == '\'' || pc == '-'))
                            return false;
                        if (c == '\'' && pc == '-')
                            return false;
                    }
                }
            }

            return true;
        }

        public async Task SaveToDatabase()
        {
            DatabaseTransaction transaction = DatabaseManager.DataCentre.BeginTransaction();

            transaction.AddPreparedStatement(DataCentreDatabase.DataCentrePreparedStatement.CharacterInsert,
                Id, AccountId, ActorId, RealmId, Name, BirthMonth, BirthDay, Guardian, Voice, ClassJobId);

            SpawnPosition.SaveToDatabase(Id, transaction);
            Appearance.SaveToDatabase(Id, transaction);

            foreach (CharacterClassInfo classInfo in classes.Where(c => c.Id != 0))
                classInfo.SaveToDatabase(Id, transaction);

            await transaction.Commit();
        }

        public string BuildJsonData()
        {
            var characterJson = new CharacterInfoJson
            {
                Content = new JArray
                {
                    Name,
                    new JArray(classes.Select(e => e.Level)),
                    0,
                    0,
                    0,
                    BirthMonth,
                    BirthDay,
                    Guardian,
                    ClassJobId,
                    0,
                    SpawnPosition.TerritoryId,
                    Appearance.Array,
                    0, // main hand
                    0, // off hand
                    new JArray
                    {
                        0, // head
                        0, // body
                        0, // hands
                        0, // legs
                        0, // feet
                        0, // ear
                        0, // neck
                        0, // wrist
                        0, // ring RH
                        0, // ring LH
                    },
                    0,
                    0,
                    7,
                    0, // flags (0x01: legacy class and edit, 0x04: edit)
                    0,
                    0,
                    "",
                    0,
                    0
                },
                ClassName = "ClientSelectData",
                ClassId   = 116
            };

            return JsonProvider.SerialiseObject(characterJson);
        }
    }
}
