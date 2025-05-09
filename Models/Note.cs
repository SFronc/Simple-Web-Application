using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

public class Note{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int noteId {get; set;}

    [Required]
    [MaxLength(100)]
    public string? title {get; set;}

    [Required]
    public string? data {get; set;}

    [Required]
    public DateTime date {get; set;}

    public int userId {get; set;}
    public User? user {get; set;}
}