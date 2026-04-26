using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL
{
    public class MediaResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        // Đường dẫn tương đối để lưu vào DB của client (ví dụ: /POS/Images/MenuItem/F001/hambuger.png)
        public string RelativeUrl { get; set; }

        // Đường dẫn tuyệt đối bao gồm cả Domain (nếu cần hiển thị ngay)
        public string FullUrl { get; set; }

        // Thông tin bổ sung để Client quản lý
        public long FileSize { get; set; }
        public string FileExtension { get; set; }

        public static MediaResponse Success(string relativeUrl, string fullUrl, long size, string ext)
        {
            return new MediaResponse
            {
                IsSuccess = true,
                RelativeUrl = relativeUrl,
                FullUrl = fullUrl,
                FileSize = size,
                FileExtension = ext
            };
        }

        public static MediaResponse Failure(string message)
        {
            return new MediaResponse { IsSuccess = false, Message = message };
        }
    }
}
