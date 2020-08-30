using MediatR;
using Ordering.Application.Commands;
using Ordering.Application.Mapper;
using Ordering.Application.Responses;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Handlers
{
    public class CheckOutorderHandler : IRequestHandler<CheckOutOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public CheckOutorderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); ;
        }

        public async Task<OrderResponse> Handle(CheckOutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = OrderMapper.Mapper.Map<Order>(request);
            if (orderEntity == null)
            {
                throw new ApplicationException("Not mapped");
            }

            Order newOrder = await _orderRepository.AddAsync(orderEntity);

            var orderResponse = OrderMapper.Mapper.Map<OrderResponse>(newOrder);

            return orderResponse;


        }
    }
}
