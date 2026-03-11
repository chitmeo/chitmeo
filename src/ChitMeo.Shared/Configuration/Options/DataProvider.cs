using System.Runtime.Serialization;

namespace ChitMeo.Shared.Configuration.Options;

public enum DataProvider
{
    [EnumMember(Value = "mariadb")]
    MariaDB,

    [EnumMember(Value = "mysql")]
    MySQL,

    [EnumMember(Value = "sqlserver")]
    SqlServer
}