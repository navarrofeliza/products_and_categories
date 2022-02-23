using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsAndCategories.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public Product Products { get; set; }
        public Category Categories { get; set; }
    }
}