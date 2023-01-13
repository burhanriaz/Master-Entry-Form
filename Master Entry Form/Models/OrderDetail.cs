using System.Data;

namespace Master_Entry_Form.Models
{
    public class OrderDetail
    {
        public int OrderDetaiId { get; set; }
        public string itemName { get; set; }
        public int Quentity { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
