using System.ComponentModel.DataAnnotations;

namespace EmpClient.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }

}
