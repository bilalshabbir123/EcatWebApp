using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModels
{
    public class EditRoleVM
    {
       public int Id { get; set; }
        [Required(ErrorMessage="Role Name is Required")]
       public string RoleName { get; set; }

        public List<string> Users { get; set; };

    }
}
