
namespace Catalog.API.Settings
{
    public interface ICatalogDatabaseSettings
    {
        string CollectionName { get; set; }
        string CollectionString { get; set; }
        string DatabaseName { get; set; }


    }
}
