using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL
{
    public class DeleteMediaRequest
    {
        public string AppCode { get; set; }
        public string MediaType { get; set; }
        public string Entity { get; set; }
        public string UniqueCode { get; set; }
        public string FileName { get; set; } // Tên file đã bao gồm Timestamp (trả về lúc Upload)
    }
}
