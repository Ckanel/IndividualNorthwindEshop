

namespace CommonData.Models
{
    public class HandleOrderViewModel
    {
        public Order Order { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public List<CustomSelectListItem> Shippers { get; set; } = new List<CustomSelectListItem>();
        public int SelectedShipperId { get; set; }
        public Dictionary<int, int> InitialReservedStock { get; set; }
    }



}
