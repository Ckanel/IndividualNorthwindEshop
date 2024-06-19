using CommonData.Models;

public class UpdatedOrderDetailViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public virtual Product Product { get; set; }
    public virtual Order Order { get; set; }
}

