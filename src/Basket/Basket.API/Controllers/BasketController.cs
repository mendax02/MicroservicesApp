using AutoMapper;
using Basket.API.Entites;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {

        private readonly IBasketRepository _repository;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _eventBus;

        public BasketController(IBasketRepository repository, IMapper mapper, EventBusRabbitMQProducer eventBus)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            return Ok(basket ?? new BasketCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basketCart)
        {
            var basket = await _repository.UpdateBasket(basketCart);

            return Ok(basket);
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var deleted = await _repository.DeleteBasket(userName);

            return Ok(deleted);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckOut([FromBody] BasketCheckOut basketCheckOut)
        {

            //Get total price of basket
            var basket = await _repository.GetBasket(basketCheckOut.UserName);

            if (basket == null)
            {
                return BadRequest();
            }
            //Remove the basket
            var basketRemoved = await _repository.DeleteBasket(basket.UserName);

            if (!basketRemoved)
            {
                return BadRequest();
            }
            // Use Auto Mapper to Map BasketCheckOut to BasketCheckOutEvent
            var eventMessage = _mapper.Map<BasketCheckOutEvent>(basketCheckOut);
            eventMessage.RequestId = Guid.NewGuid();

            eventMessage.TotalPrice = basket.TotalPrice;
            // Send checkout event to RabbitMQ
            try
            {

                _eventBus.PublishBasketCheckOut(EventBusConstants.BasketCheckOutQueue, eventMessage);
            }
            catch (Exception)
            {

                throw;
            }

            return Accepted();
        }
    }
}
