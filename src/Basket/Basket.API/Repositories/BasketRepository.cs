using Basket.API.Data.Interfaces;
using Basket.API.Entites;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _context
                            .Redis
                            .StringGetAsync(userName);

            if (basket.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basketCart)
        {
            var updated = await _context
                                        .Redis
                                        .StringSetAsync(basketCart.UserName, JsonConvert.SerializeObject(basketCart));
            if (!updated)
            {
                return null;
            }

            return await GetBasket(basketCart.UserName);
        }
        public async Task<bool> DeleteBasket(string userName)
        {
            return await _context
                          .Redis
                          .KeyDeleteAsync(userName);
        }


    }
}
