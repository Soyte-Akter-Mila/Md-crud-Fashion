#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MD_Crud_Fashion.Models;

namespace MD_Crud_Fashion.Models.ViewModels
{
    public class CustomerVM
    {
        public int CustomersId { get; set; }

        [Display(Name = "Customers Name"), Required]
        public string? CustomersName { get; set; }

        [Display(Name = "Payment Date"), Required,
         Column(TypeName = "date"),
         DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Dress Size")]
        public int CustomerSize { get; set; }

        public string? Picture { get; set; }

        [Display(Name = "Upload Picture")]
        public IFormFile? PictureFile { get; set; }

        [Display(Name = "Urgent Delivery")]
        public bool UrgentDelivery { get; set; }

        public ICollection<OrderEntry>? OrderEntries { get; set; } = new List<OrderEntry>();

        public List<int> ProductList { get; set; } = new List<int>();
    }
}
