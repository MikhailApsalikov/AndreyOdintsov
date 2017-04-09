namespace Models
{
	using System.ComponentModel.DataAnnotations;

	public class LogOnModel
    {
        [Required]
        [Display(Name = "Логин SAP")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}