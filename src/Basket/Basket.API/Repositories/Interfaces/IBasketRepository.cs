﻿using Basket.API.Entites;
using System.Threading.Tasks;

namespace Basket.API.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<BasketCart> GetBasket(string userName);

        Task<BasketCart> UpdateBasket(BasketCart basketCart);

        Task<bool> DeleteBasket(string userName);
    }
}
