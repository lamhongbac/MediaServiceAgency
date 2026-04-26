using MSA.DAL.Models.MSA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSA.DAL.Repos
{
    /// <summary>
    /// Giao diện định nghĩa các thao tác truy cập dữ liệu cho bảng AppPartners.
    /// </summary>
    public interface IAppRepository
    {
        /// <summary>
        /// Lấy thông tin ứng dụng dựa trên AppCode.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng cần tìm.</param>
        /// <returns>Đối tượng AppPartner nếu tìm thấy, ngược lại trả về null.</returns>
        Task<AppPartner> GetByAppCodeAsync(string appCode);

        /// <summary>
        /// Thêm mới một ứng dụng vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="appModel">Dữ liệu ứng dụng cần lưu.</param>
        /// <returns>True nếu thêm thành công.</returns>
        Task<bool> InsertAppAsync(AppPartner appModel);

        /// <summary>
        /// Kiểm tra sự tồn tại của AppCode trong hệ thống.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng cần kiểm tra.</param>
        /// <returns>True nếu đã tồn tại.</returns>
        Task<bool> IsAppCodeExistAsync(string appCode);

        /// <summary>
        /// Cập nhật trạng thái hoạt động (Active/Deactive) của ứng dụng.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng.</param>
        /// <param name="isActive">Trạng thái mới.</param>
        /// <returns>True nếu cập nhật thành công.</returns>
        Task<bool> UpdateStatusAsync(string appCode, bool isActive);
    }
}
