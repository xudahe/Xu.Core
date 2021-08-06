using Xu.EventBus;

namespace Xu.Extensions.EventHandling
{
    /// <summary>
    /// 事件源的声明
    /// </summary>
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; private set; }

        public ProductPriceChangedIntegrationEvent(int productId)
            => ProductId = productId;
    }
}