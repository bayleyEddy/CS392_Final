using CS392_Demo3.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CS392_Demo3.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Product> _products;

        public MongoDBService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDBSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDBSettings:DatabaseName"]);
            _products = database.GetCollection<Product>(configuration["MongoDBSettings:CollectionName"]);
        }

        // BASIC CRUD
        public async Task<List<Product>> GetAllAsync() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<Product> GetAsync(string id) =>
            await _products.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Product product) =>
            await _products.InsertOneAsync(product);

        public async Task UpdateAsync(string id, Product product) =>
            await _products.ReplaceOneAsync(x => x.Id == id, product);

        public async Task DeleteAsync(string id) =>
            await _products.DeleteOneAsync(x => x.Id == id);

        // FILTERING FOR CHATBOT
        public async Task<List<Product>> FilterProductsAsync(string category, double? maxPrice, bool? inStock)
        {
            var filter = Builders<Product>.Filter.Empty;

            // Case-insensitive partial match for category
            if (!string.IsNullOrEmpty(category))
            {
                filter &= Builders<Product>.Filter.Regex(
                    p => p.category,
                    new MongoDB.Bson.BsonRegularExpression(category, "i")
                );
            }

            if (maxPrice.HasValue)
                filter &= Builders<Product>.Filter.Lte(p => p.price, maxPrice.Value);

            if (inStock.HasValue)
                filter &= Builders<Product>.Filter.Eq(p => p.inStock, inStock.Value);

            return await _products.Find(filter).ToListAsync();
        }
    }
}
