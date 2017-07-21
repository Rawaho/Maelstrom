using System;
using System.Collections.Generic;
using Shared.Game;

namespace Shared.Database.World
{
    public class WorldDatabase : Database
    {
        private enum WorldPreparedStatement
        {
            CharacterSpawnSelect
        }

        public override Type PreparedStatementType => typeof(WorldPreparedStatement);

        protected override void InitialisePreparedStatements()
        {
            AddPreparedStatement(WorldPreparedStatement.CharacterSpawnSelect, "SELECT `cityStateId`, `territoryId`, `x`, `y`, `z`, `o` FROM `character_spawn`;");
        }

        public IEnumerable<(byte CityStateId, WorldPosition Position)> GetCharacterSpawns()
        {
            MySqlResult result = SelectPreparedStatement(WorldPreparedStatement.CharacterSpawnSelect);
            for (uint i = 0; i < result?.Count; i++)
                yield return (result.Read<byte>(i, "cityStateId"), new WorldPosition(result.Rows[(int)i]));
        }
    }
}
