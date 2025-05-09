namespace WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int userId {get; set;}

    [Required]
    public string? login {get; set;}

    [Required]
    public string? password {get; set;}

}