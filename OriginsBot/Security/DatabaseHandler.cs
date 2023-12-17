using MySql.Data.MySqlClient;

namespace OriginsBot.Security;

public class DatabaseHandler(IConfiguration configuration)
{
    public static MySqlConnection Connection { get; private set; } = null!;
    
    public void Initialize()
    {
        Connection =  new MySqlConnection($"Server={configuration["dbAddress"]}; Port={configuration["dbPort"]}; Database={configuration["dbDatabase"]}; Uid={configuration["dbUsername"]}; Pwd={configuration["dbPassword"]};");
    }
}