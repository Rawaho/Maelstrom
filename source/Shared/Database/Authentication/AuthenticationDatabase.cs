using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Shared.Database.Authentication
{
    public class AuthenticationDatabase : Database
    {
        private enum AuthenticationPreparedStatement
        {
            AccountSelect,
            AccountInsert,
            AccountServiceSelect,
            AccountServiceInsert,
            VersionSelect
        }

        public override Type PreparedStatementType => typeof(AuthenticationPreparedStatement);

        protected override void InitialisePreparedStatements()
        {
            AddPreparedStatement(AuthenticationPreparedStatement.AccountSelect, "SELECT `id` FROM `account` WHERE `sessionId` = ?;",
                MySqlDbType.VarChar);
            AddPreparedStatement(AuthenticationPreparedStatement.AccountInsert, "INSERT INTO `account` (`name`, `password`, `salt`, `sessionId`) VALUES (?, ?, ?, ?);",
                MySqlDbType.VarChar, MySqlDbType.VarChar, MySqlDbType.VarChar, MySqlDbType.VarChar);

            AddPreparedStatement(AuthenticationPreparedStatement.AccountServiceSelect, "SELECT `id`, `name`, `expansion`, `realmCharacterLimit`, "
                + "`accountCharacterLimit` FROM `account_service` WHERE `accountId` = ?",
                MySqlDbType.UInt32);
            AddPreparedStatement(AuthenticationPreparedStatement.AccountServiceInsert, "INSERT INTO `account_service` (`accountId`, `name`) VALUES (?, ?);",
                MySqlDbType.UInt32, MySqlDbType.VarChar);

            AddPreparedStatement(AuthenticationPreparedStatement.VersionSelect, "SELECT `file`, `version`, `digest` FROM `version`;");
        }

        public bool CreateAccount(string username, string password, string salt)
        {
            // TODO: set session id to username till launcher is created
            return ExecutePreparedStatement(AuthenticationPreparedStatement.AccountInsert, username, password, salt, username) == 1;
        }

        public async Task<uint> GetAccount(string sessionId)
        {
            if (sessionId == string.Empty)
                return 0u;

            MySqlResult result = await SelectPreparedStatementAsync(AuthenticationPreparedStatement.AccountSelect, sessionId);
            return result.Count != 0u ? result.Read<uint>(0, "id") : 0u;
        }

        public bool CreateServiceAccount(uint accountId, string name)
        {
            return ExecutePreparedStatement(AuthenticationPreparedStatement.AccountServiceInsert, accountId, name) == 1;
        }

        public async Task<List<ServiceAccountInfo>> GetServiceAccounts(uint accountId)
        {
            var serviceAccountList = new List<ServiceAccountInfo>();

            MySqlResult result = await SelectPreparedStatementAsync(AuthenticationPreparedStatement.AccountServiceSelect, accountId);
            for (int i = 0; i < result?.Count; i++)
                serviceAccountList.Add(new ServiceAccountInfo(result.Rows[i]));

            return serviceAccountList;
        }

        public IEnumerable<VersionInfo> LoadVersionInfo()
        {
            MySqlResult result = SelectPreparedStatement(AuthenticationPreparedStatement.VersionSelect);
            for (int i = 0; i < result?.Count; i++)
                yield return new VersionInfo(result.Rows[i]);
        }
    }
}
