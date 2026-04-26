using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL
{
    // Cấu trúc Request lưu trữ Media (Không chứa IFormFile để đảm bảo tính độc lập)
    public class UploadMediaRequest
    {
        public string AppCode { get; set; }
        public string MediaType { get; set; }
        public string Entity { get; set; }
        public string UniqueCode { get; set; }
    }
}
