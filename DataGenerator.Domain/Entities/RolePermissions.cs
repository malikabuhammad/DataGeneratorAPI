using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Entities
{
    public class RolePermissions
    {
        [Key]
        public int RoleID { get; set; }
        public int PermissionID { get; set; }
        public int ID { get; set; }
    }
}
