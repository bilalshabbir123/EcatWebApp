using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModels
{
    public  class EditUserVM
    {
        public EditUserVM()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }

        public List<string>  Claims{ get; set; }
        public IList<string> Roles { get; set; }
    }
}
