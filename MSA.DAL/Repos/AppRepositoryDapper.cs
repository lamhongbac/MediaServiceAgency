using MSA.DAL.Models.MSA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MSA.DAL.Repos
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho bảng AppPartners sử dụng Dapper.
    /// </summary>
    public class AppRepositoryDapper : IAppRepository
    {
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// Khởi tạo repository với đối tượng IDbConnection đã được cấu hình.
        /// </summary>
        /// <param name="dbConnection">Connection tới SQL Server.</param>
        public AppRepositoryDapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Lấy thông tin ứng dụng theo mã AppCode (chỉ lấy các ứng dụng đang Active).
        /// </summary>
        /// <param name="appCode">Mã ứng dụng.</param>
        /// <returns>Đối tượng AppPartner nếu tìm thấy.</returns>
        public async Task<AppPartner> GetByAppCodeAsync(string appCode)
        {
            string sql = "SELECT * FROM AppPartners WHERE AppCode = @AppCode AND IsActive = 1";
            return await _dbConnection.QueryFirstOrDefaultAsync<AppPartner>(sql, new { AppCode = appCode });
        }

        /// <summary>
        /// Kiểm tra xem AppCode đã tồn tại trong DB chưa.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng cần kiểm tra.</param>
        /// <returns>True nếu đã tồn tại ít nhất 1 bản ghi.</returns>
        public async Task<bool> IsAppCodeExistAsync(string appCode)
        {
            string sql = "SELECT COUNT(1) FROM AppPartners WHERE AppCode = @AppCode";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { AppCode = appCode });
            return count > 0;
        }
        
        /// <summary>
        /// Thêm mới thông tin đăng ký ứng dụng vào bảng AppPartners.
        /// </summary>
        /// <param name="model">Dữ liệu model AppPartner.</param>
        /// <returns>True nếu chèn thành công bản ghi.</returns>
        public async Task<bool> InsertAppAsync(AppPartner model)
        {            

            string sql = @"INSERT INTO AppPartners (AppCode, ApiKey, AppName, IsActive, CreatedDate) 
                   VALUES (@AppCode, @ApiKey, @AppName, @IsActive, @CreatedDate)";

            try
            {
                // Dapper sẽ tự động bóc tách các thuộc tính của 'model' 
                // để khớp với các @Parameter trong câu lệnh SQL.
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, model);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log lỗi (ví dụ: vi phạm ràng buộc Unique AppCode)
                return false;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái Active/Deactive cho ứng dụng đối tác.
        /// </summary>
        /// <param name="appCode">Mã ứng dụng.</param>
        /// <param name="isActive">Trạng thái cần cập nhật.</param>
        /// <returns>True nếu có bản ghi bị ảnh hưởng.</returns>
        public async Task<bool> UpdateStatusAsync(string appCode, bool isActive)
        {
            string sql = "UPDATE AppPartners SET IsActive = @IsActive WHERE AppCode = @AppCode";
            int rows = await _dbConnection.ExecuteAsync(sql, new { IsActive = isActive, AppCode = appCode });
            return rows > 0;
        }

        
    }
}
