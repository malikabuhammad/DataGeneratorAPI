using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Entities
{
    public class Comments
    {
        [Key]
        public long CommentID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string Description { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
