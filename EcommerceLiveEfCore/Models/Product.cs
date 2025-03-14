﻿using System.ComponentModel.DataAnnotations;

namespace EcommerceLiveEfCore.Models
{
    public class Product
    {
        //definiamo le proprietà che rappresentano le colonne della tabella sul database
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public required string Name { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public required string Description { get; set; }

        [Required]
        [Range(1, 5000)]
        public double Price { get; set; }

        [Required]
        public required string Category { get; set; }
    }
}
