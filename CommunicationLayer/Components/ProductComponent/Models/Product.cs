using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string ProductSlug { get; set; }
        [Column(TypeName = "text")]
        public string Description { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public double Price { get; set; }
        public string MetaTags { get; set; }
    }
}
