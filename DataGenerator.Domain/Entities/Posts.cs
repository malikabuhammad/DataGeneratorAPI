using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Entities
{
    public class Posts
    {
        [Key]
        public long PostID { get; set; }
        public string Title { get; set; }
        public string Post_Content { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsPublished { get; set; }
        public bool Is_EditorPick { get; set; }
        public string ImgPath { get; set; }
        public int Views { get; set; }
    }
}
