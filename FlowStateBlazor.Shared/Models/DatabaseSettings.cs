namespace FlowStateBlazor.Shared.Models
{
    public class DatabaseSettings
    {
        public const string DatabaseSettingsSectionName = "Database";

        public string DatabaseType { get; set; } = "SQLSERVER";
    }
}
