using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eStore.Service.Models.RequestModels
{
    public class CreateOrderRequestModel
    {
        public int? MemberId { get; set; }

        public DateTime? RequiredDate { get; set; }
        public virtual ICollection<OrderDetailRequestModel> OrderDetails { get; set; }
    }

    public class OrderDetailRequestModel
    {
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }
    }
}
