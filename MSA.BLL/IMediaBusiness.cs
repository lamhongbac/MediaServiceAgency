using MSA.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.BLL
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ cốt lõi của Media Service (Đăng ký, Lưu trữ, Xóa, Xác thực).
    /// </summary>
    public interface IMediaBusiness
    {
        /// <summary>
        /// Đăng ký một ứng dụng đối tác mới.
        /// </summary>
        /// <param name="dto">Thông tin ứng dụng đăng ký.</param>
        /// <returns>Thông tin kết quả đăng ký bao gồm ApiKey.</returns>
        Task<RegisterResponseDto> RegisterNewAppAsync(RegisterAppDto dto);

        /// <summary>
        /// Lưu trữ tệp tin media vào hệ thống lưu trữ vật lý.
        /// </summary>
        /// <param name="dto">Metadata thông tin file.</param>
        /// <param name="fileStream">Luồng dữ liệu của tệp tin.</param>
        /// <param name="fileName">Tên tệp tin gốc.</param>
        /// <returns>Thông tin kết quả lưu trữ (URL, Size, v.v.)</returns>
        Task<MediaResponse> SaveMediaAsync(UploadMediaRequest dto, Stream fileStream, string fileName);        
        
        /// <summary>
        /// Kiểm tra tính hợp lệ của AppCode và ApiKey thông qua tầng DAL.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng.</param>
        /// <param name="apiKey">Mã bảo mật.</param>
        /// <returns>True nếu hợp lệ và ứng dụng đang hoạt động.</returns>
        Task<bool> ValidateApiKeyAsync(string appCode, string apiKey);       
        
        /// <summary>
        /// Xóa tệp tin media vật lý dựa trên các tiêu chí Metadata.
        /// </summary>
        /// <param name="request">Metadata định danh file cần xóa.</param>
        /// <returns>True nếu xóa thành công ít nhất một tệp tin.</returns>
        Task<bool> DeleteMediaAsync(DeleteMediaRequest request);
    }
}
