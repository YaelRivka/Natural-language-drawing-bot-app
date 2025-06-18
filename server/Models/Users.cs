using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }


}
