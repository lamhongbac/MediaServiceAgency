using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.DAL.Models
{
    namespace MSA.DAL.Models
    {
        public class AppPartner
        {
            public int Id { get; set; }
            public string AppCode { get; set; } // Ví dụ: "POS"
            public string ApiKey { get; set; }  // Chuỗi bí mật
            public string AppName { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}
