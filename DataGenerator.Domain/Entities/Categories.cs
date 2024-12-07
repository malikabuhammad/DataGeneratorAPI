using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Entities
{
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryNameAr { get; set; }
        public string CategoryNameEn { get; set; }
    }
}
