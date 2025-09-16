using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public partial class Validation
{
    [Key]
    public string Token { get; set; }
}
