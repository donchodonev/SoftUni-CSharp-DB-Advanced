﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Input

{
    [XmlType("Product")]
    public class ProductsInputDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]

        public decimal Price { get; set; }

        [XmlElement("sellerId")]

        public int SellerId { get; set; }

        [XmlElement("buyerId")]

        public int? BuyerId { get; set; }
    }
}
