using dapperCRUD.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace dapperCRUD.Models
{
    public class Customer
    {
                
        public int Id { get; set; }

        [Required(ErrorMessage = "name required")]
        [MinLength(2)]
        [MaxLength(255)]
        public string Name { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }


        
        [DataType(DataType.Date)]
        [CustomerDateOfBirthValidation]
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
    }

}
