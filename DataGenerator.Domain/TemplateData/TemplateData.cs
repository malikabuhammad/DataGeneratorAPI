using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.TemplateData
{
    public class TemplateData
    {
        public string TableName { get; set; }
        public string EntityName { get; set; }
        public IEnumerable<ColumnMeta> Columns { get; set; }
        public string Namespace { get; set; }

        public int PermIndex { get; set; }
        public int PermAdd => PermIndex + 1;
        public int PermEdit => PermIndex + 2;
        public int PermDelete => PermIndex + 3;
    }
    public class ColumnMeta
    {
         public string column_name { get; set; }
        public string data_type { get; set; }
        public bool IsIdentity { get; set; }
    }

}
