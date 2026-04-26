using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL
{
    public class UploadRequestDto
    {
        public string AppCode { get; set; }    // POS
        public string MediaType { get; set; }  // Images
        public string Entity { get; set; }     // MenuItem
        public string UniqueCode { get; set; } // F001
        public IFormFile File { get; set; }    // File vật lý
    }
}
