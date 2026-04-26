using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL.BusinessObjects
{
    public class AppPartnerBO
    {
        public int Id { get; set; }
        public string AppCode { get; set; } // Ví dụ: "POS"
        public string ApiKey { get; set; }  // Chuỗi bí mật
        public string AppName { get; set; }
        public string IsActive { get; set; } //actived or Locked
        public DateTime CreatedDate { get; set; }
    }
}
