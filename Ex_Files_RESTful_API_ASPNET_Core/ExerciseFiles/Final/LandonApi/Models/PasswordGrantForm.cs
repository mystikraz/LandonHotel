using LandonApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    /// <summary>
    /// This class is not used directly; it is used to serialize the metadata
    /// and describe an OpenID Connect Password Grant request to the client.
    /// </summary>
    public class PasswordGrantForm
    {
        [Required]
        [Display(Name = "grant_type")]
        public string GrantType { get; set; } = "password";

        [Required]
        [Display(Name = "username", Description = "Email address")]
        public string Username { get; set; }

        [Required, Secret]
        [Display(Name = "password", Description = "Password")]
        public string Password { get; set; }
    }
}
