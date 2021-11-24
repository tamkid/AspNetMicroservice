using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService, IMapper mapper, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, 
                                        CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            var orderAdded = await _orderRepository.AddAsync(order);
            _logger.LogInformation($"Order {orderAdded.Id} was created successfully.");

            /* Send Email */
            await SendEmail(orderAdded);

            return orderAdded.Id;
        }

        private async Task SendEmail(Order orderAdded)
        {
            try
            {
                var email = new Email
                { 
                    To = "thedragonkid01@gmail.com",
                    Subject = "Order Creating",
                    Body = $"Order {orderAdded.Id} was created successfully."
                };
                await _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sending email creating order successfully fails: {ex.Message}");
            }
        }
    }
}
