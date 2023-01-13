namespace Master_Entry_Form.Models.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime Date { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
