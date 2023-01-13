namespace Master_Entry_Form.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo{ get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
