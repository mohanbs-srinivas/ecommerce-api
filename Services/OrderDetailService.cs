using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.Models;

namespace ecommerce_api.Services
{
    public class OrderDetailService
    {
        private readonly List<OrderDetail> _orderDetails;

        public OrderDetailService()
        {
            _orderDetails = new List<OrderDetail>();
        }

        public Task<List<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return Task.FromResult(_orderDetails);
        }

        public Task<OrderDetail> GetOrderDetailByIdAsync(int id)
        {
            var orderDetail = _orderDetails.FirstOrDefault(od => od.Id == id);
            return Task.FromResult(orderDetail);
        }

        public Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            orderDetail.Id = _orderDetails.Count > 0 ? _orderDetails.Max(od => od.Id) + 1 : 1;
            _orderDetails.Add(orderDetail);
            return Task.FromResult(orderDetail);
        }

        public Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            var existingOrderDetail = _orderDetails.FirstOrDefault(od => od.Id == orderDetail.Id);
            if (existingOrderDetail != null)
            {
                existingOrderDetail.OrderId = orderDetail.OrderId;
                existingOrderDetail.ProductId = orderDetail.ProductId;
                existingOrderDetail.Quantity = orderDetail.Quantity;
                existingOrderDetail.Price = orderDetail.Price;
            }
            return Task.FromResult(existingOrderDetail);
        }

        public Task<bool> DeleteOrderDetailAsync(int id)
        {
            var orderDetail = _orderDetails.FirstOrDefault(od => od.Id == id);
            if (orderDetail != null)
            {
                _orderDetails.Remove(orderDetail);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}