using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [Required]
        public string Name { get; set; }

        public string Pseudonym { get; set; }

        public string Text { get; set; }

        public string PhoneNumber { get; set; }

        public ICollection<Album> Albums { get; set; }
    }
}
