using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Input

{
    [XmlRoot]
    [XmlType("User")]
    public class UsersInputDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public string Age { get; set; }
    }
}
