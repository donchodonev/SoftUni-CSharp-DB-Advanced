namespace FastFood.Core.ViewModels.Orders
{
    using System.Collections.Generic;

    public class CreateOrderViewModel
    {
        public CreateOrderViewModel()
        {
            Items = new List<int>();
            Employees = new List<int>();
        }
        public List<int> Items { get; set; }

        public List<int> Employees { get; set; }
    }
}
