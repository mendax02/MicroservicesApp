using AutoMapper;
using Basket.API.Entites;
using EventBusRabbitMQ.Events;

namespace Basket.API.Mapping
{
    public class BasketMapping: Profile
    {
        public BasketMapping()
        {
            CreateMap<BasketCheckOut, BasketCheckOutEvent>().ReverseMap();
        }
    }
}
