using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        private readonly ICatalogDatabaseSettings _settings;

        public CatalogContext(ICatalogDatabaseSettings settings)
        {
            _settings = settings ?? throw new NullReferenceException();

            var client = new MongoClient(_settings.CollectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            Products = database.GetCollection<Product>(_settings.CollectionName);
        }

        public IMongoCollection<Product> Products { get; }
    }
}
