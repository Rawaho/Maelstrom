using Shared.Database.Authentication;
using Shared.Database.Datacentre;
using Shared.Database.World;

namespace Shared.Database
{
    public static class DatabaseManager
    {
        public static AuthenticationDatabase Authentication { get; } = new AuthenticationDatabase();
        public static DataCentreDatabase DataCentre { get; } = new DataCentreDatabase();
        public static WorldDatabase World { get; } = new WorldDatabase();

        public static void Initialise(ConfigMySqlDatabase authentication = null, ConfigMySqlDatabase dataCentre = null, ConfigMySqlDatabase world = null)
        {
            if (authentication != null)
                Authentication.Initialise(authentication.Host, authentication.Port, authentication.Username, authentication.Password, authentication.Database);

            if (dataCentre != null)
                DataCentre.Initialise(dataCentre.Host, dataCentre.Port, dataCentre.Username, dataCentre.Password, dataCentre.Database);

            if (world != null)
                World.Initialise(world.Host, world.Port, world.Username, world.Password, world.Database);
        }
    }
}
