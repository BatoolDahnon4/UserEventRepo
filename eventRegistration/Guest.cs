using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eventRegistration
{
    public class Guest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Position { get; set; } = String.Empty;
        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(254)")]
        public string Email { get; set; }
        public string CompanyName { get; set; } = String.Empty;
        public string PhoneNumber { get; set; }
        public string Source { get; set;} = string.Empty;
        public Boolean IsAttended { get; set; }
        public string Okay { get; set; } = string.Empty;
        public string Table { get; set; }=string.Empty;

       
    }
}
