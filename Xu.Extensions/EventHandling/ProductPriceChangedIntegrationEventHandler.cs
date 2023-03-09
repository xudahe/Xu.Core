using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xu.Common;
using Xu.EventBus;
using Xu.IServices;

namespace Xu.Extensions.EventHandling
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IUserSvc _userSvc;
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;

        public ProductPriceChangedIntegrationEventHandler(
            IUserSvc userSvc,
            ILogger<ProductPriceChangedIntegrationEventHandler> logger)
        {
            _userSvc = userSvc;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 定义事件处理
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "Blog.Core", @event);

            ConsoleHelper.WriteSuccessLine($"----- Handling integration event: {@event.Id} at Xu.Core - ({@event})");

            await _userSvc.GetUserNameById(@event.ProductId);
        }
    }
}