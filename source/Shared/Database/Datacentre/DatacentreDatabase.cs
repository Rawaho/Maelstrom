using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Shared.Database.Datacentre
{
    public class DataCentreDatabase : Database
    {
        public enum DataCentrePreparedStatement
        {
            RealmListSelect,
            CharacterNameCount,
            CharacterSelect,
            CharacterClassSelect,
            CharacterInsert,
            CharacterAppearanceInsert,
            CharacterPositionInsert,
            CharacterClassInsert,
            CharacterMaxSelect,
            CharacterSessionUpdate,
            CharacterSessionSelect
        }

        public override DatabaseTransaction BeginTransaction() { return new DatabaseTransaction(this); }

        public override Type PreparedStatementType => typeof(DataCentrePreparedStatement);

        protected override void InitialisePreparedStatements()
        {
            AddPreparedStatement(DataCentrePreparedStatement.RealmListSelect,
                "SELECT `id`, `name`, `flags`, `host`, `port` FROM `realm_list`;");

            AddPreparedStatement(DataCentrePreparedStatement.CharacterNameCount,
                "SELECT COUNT(`name`) as `count` FROM `character` WHERE `name` = ?;",
                MySqlDbType.VarChar);

            // character
            AddPreparedStatement(DataCentrePreparedStatement.CharacterMaxSelect,
                "SELECT MAX(`id`) AS `max` FROM `character`;");

            AddPreparedStatement(DataCentrePreparedStatement.CharacterSelect,
                "SELECT `character`.`id`, `serviceAccountId`, `actorId`, `realmId`, `name`, `race`, `sex`, `birthMonth`, `birthDay`, `guardian`, `voice`, "
                + "`ClassJobId`, `height`, `clan`, `bustSize`, `skinColour`, `tailShape`, `tailLength`,`hairStyle`, `hairColour`, `hairColourHighlights`, "
                + "`face`, `jaw`, `eye`, `eyeColour`, `eyeColourOdd`, `eyebrows`, `nose`, `mouth`, `lipColour`, `facialFeatures`, `tattooColour`, `facePaint`, "
                + "`facePaintColour`, `territoryId`, `x`, `y`, `z`, `o` FROM `character` INNER JOIN `character_appearance` ON `serviceAccountId` = ? AND "
                + "`character`.`id` = `character_appearance`.`Id` INNER JOIN `character_position` ON `character`.`id` = `character_position`.`id`;",
                MySqlDbType.UInt32);

            AddPreparedStatement(DataCentrePreparedStatement.CharacterClassSelect, "SELECT `classId`, `level`, `xp` FROM `character_class` WHERE `id` = ?;",
                MySqlDbType.UInt64);

            // character
            AddPreparedStatement(DataCentrePreparedStatement.CharacterInsert,
                "INSERT INTO `character` (`id`, `serviceAccountId`, `actorId`, `realmId`, `name`, `birthMonth`, `birthDay`, `guardian`, `voice`, `ClassJobId`) "
                + "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);",
                MySqlDbType.UInt64, MySqlDbType.UInt32, MySqlDbType.UInt32, MySqlDbType.UInt16, MySqlDbType.VarChar, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, 
                MySqlDbType.UByte, MySqlDbType.UByte);

            AddPreparedStatement(DataCentrePreparedStatement.CharacterAppearanceInsert,
                "INSERT INTO `character_appearance` (`id`, `race`, `sex`, `height`, `clan`, `bustSize`, `skinColour`, `tailShape`, `tailLength`, `hairStyle`, `hairColour`, "
                + "`hairColourHighlights`, `face`, `jaw`, `eye`, `eyeColour`, `eyeColourOdd`, `eyebrows`, `nose`, `mouth`, `lipColour`, `facialFeatures`, `tattooColour`, `facePaint`, "
                + "`facePaintColour`) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);",
                MySqlDbType.UInt64, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte,
                MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte,
                MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte);

            AddPreparedStatement(DataCentrePreparedStatement.CharacterPositionInsert,
                "INSERT INTO `character_position` (`id`, `territoryId`, `x`, `y`, `z`, `o`) VALUES (?, ?, ?, ?, ?, ?)",
                MySqlDbType.UInt64, MySqlDbType.UInt16, MySqlDbType.Float, MySqlDbType.Float, MySqlDbType.Float, MySqlDbType.Float);

            AddPreparedStatement(DataCentrePreparedStatement.CharacterClassInsert,
                "INSERT INTO `character_class` (`id`, `classId`, `level`, `xp`) VALUES (?, ?, ?, ?);",
                MySqlDbType.UInt64, MySqlDbType.UByte, MySqlDbType.UByte, MySqlDbType.UByte);

            // session
            AddPreparedStatement(DataCentrePreparedStatement.CharacterSessionUpdate,
                "UPDATE `character` SET `sessionSource` = ? WHERE `id` = ?;",
                MySqlDbType.VarChar, MySqlDbType.UInt64);

            AddPreparedStatement(DataCentrePreparedStatement.CharacterSessionSelect,
                "SELECT `id`, `serviceAccountId` FROM `character` WHERE `actorId` = ? AND `sessionSource` = ?;",
                MySqlDbType.UInt32, MySqlDbType.VarChar);
        }

        public IEnumerable<RealmInfo> GetRealmList()
        {
            MySqlResult result = SelectPreparedStatement(DataCentrePreparedStatement.RealmListSelect);
            for (int i = 0; i < result?.Count; i++)
                yield return new RealmInfo(result.Rows[i]);
        }

        public ulong GetMaxCharacterId()
        {
            MySqlResult result = SelectPreparedStatement(DataCentrePreparedStatement.CharacterMaxSelect);
            return result?.Read<ulong>(0, "max") ?? 0ul;
        }
        
        public async Task<List<CharacterInfo>> GetCharacters(uint serviceAccountId)
        {
            var characters = new List<CharacterInfo>();

            MySqlResult result = await SelectPreparedStatementAsync(DataCentrePreparedStatement.CharacterSelect, serviceAccountId);
            for (int i = 0; i < result?.Count; i++)
            {
                var characterInfo = new CharacterInfo(result.Rows[i]);

                MySqlResult levelResult = await SelectPreparedStatementAsync(DataCentrePreparedStatement.CharacterClassSelect, characterInfo.Id);
                for (int j = 0; j < levelResult?.Count; j++)
                    characterInfo.AddClassInfo(levelResult.Rows[j]);

                characters.Add(characterInfo);
            }

            return characters;
        }

        public async Task<bool> IsCharacterNameAvailable(string name)
        {
            MySqlResult result = await SelectPreparedStatementAsync(DataCentrePreparedStatement.CharacterNameCount, name);
            return result?.Read<uint>(0, "count") == 0u;
        }

        public async Task CreateCharacterSession(ulong id, string source)
        {
            await ExecutePreparedStatementAsync(DataCentrePreparedStatement.CharacterSessionUpdate, source, id);
        }

        public async Task<(uint ServiceAccountId, ulong CharacterId)> GetCharacterSession(uint actorId, string source)
        {
            MySqlResult result = await SelectPreparedStatementAsync(DataCentrePreparedStatement.CharacterSessionSelect, actorId, source);
            return result?.Count > 0u ? (result.Read<uint>(0, "serviceAccountId"), result.Read<ulong>(0, "id")) : (0u, 0ul);
        }
    }
}
