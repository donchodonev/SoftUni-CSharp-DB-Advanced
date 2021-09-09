using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Input

{
    [XmlType("CategoryProduct")]
    public class CategoriesProductsInputDTO
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
    }
}
