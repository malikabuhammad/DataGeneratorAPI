using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Entities
{
    public class PostCategories
    {
        [Key]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int PostID { get; set; }

    }
}
