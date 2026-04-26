using Microsoft.Extensions.Configuration;
using MSA.BLL.DTOs;
using MSA.DAL.Models.MSA.DAL.Models;
using MSA.DAL.Repos;
using System.Linq;

namespace MSA.BLL
{
    //wwwroot/{AppCode}/{MediaType}/{Entity}/{UniqueCode}/{FileName}.
    /// <summary>
    /// Lớp xử lý nghiệp vụ chính cho Media Service, bao gồm lưu trữ vật lý, xóa và xác thực ứng dụng.
    /// </summary>
    /// <summary>
    /// Lớp xử lý nghiệp vụ chính cho Media Service, bao gồm lưu trữ vật lý, xóa và xác thực ứng dụng.
    /// </summary>
    public class MediaBusiness : IMediaBusiness
    {
        
        private readonly IAppRepository _appRepository;
        private readonly string _storageRoot;
        private readonly string _baseDomain;

        /// <summary>
        /// Khởi tạo MediaBusiness với các tham số cấu hình.
        /// </summary>
        /// <remarks>
        /// <b>Setup:</b> Các giá trị cấu hình được lấy từ appsettings.json:
        /// - <i>MediaConfig:StorageRoot</i>: Thư mục gốc lưu file (ví dụ: "D:\\MSA_Storage"). Cần setup khi triển khai lên Server để tách biệt data khỏi code.
        /// - <i>MediaConfig:BaseDomain</i>: Domain của CDN (ví dụ: "https://cdn.msa.com"). Dùng để trả về Full URL cho Client.
        /// </remarks>
        /// <param name="config">Cấu hình hệ thống (appsettings.json).</param>
        /// <param name="appRepository">Repository quản lý ứng dụng đối tác.</param>
        public MediaBusiness(IConfiguration config, IAppRepository appRepository)
        {
            // Lấy đường dẫn từ appsettings, nếu không có thì mặc định vào một folder an toàn
            _storageRoot = config["MediaConfig:StorageRoot"] ?? @"C:\MSA_Storage";
            _baseDomain = config["MediaConfig:BaseDomain"] ?? "";
            _appRepository = appRepository;
        }

        /// <summary>
        /// Lưu trữ tệp tin media vào thư mục vật lý theo cấu trúc phân cấp.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Hàm này tự động tạo cấu trúc thư mục nếu chưa tồn tại. 
        /// Luôn sử dụng Stream để đọc file nhằm tránh lỗi Memory khi xử lý video hoặc ảnh dung lượng lớn.
        /// </remarks>
        /// <param name="dto">Metadata bao gồm AppCode, MediaType, Entity, UniqueCode.</param>
        /// <param name="fileStream">Luồng dữ liệu file.</param>
        /// <param name="fileName">Tên file gốc để lấy Extension.</param>
        /// <returns>Đối tượng MediaResponse chứa URL và thông tin file đã lưu.</returns>
        public async Task<MediaResponse> SaveMediaAsync(UploadMediaRequest dto, Stream fileStream, string fileName)
        {
            try
            {
                // 1. Xử lý Extension và chuẩn hóa về chữ thường (Lower case)
                string ext = Path.GetExtension(fileName).ToLower();

                // 2. Tạo tên file mới: {UniqueCode}_{Timestamp} 
                // Sử dụng Ticks hoặc định dạng yyyyMMddHHmmss để đảm bảo tính duy nhất
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string safeFileName = $"{dto.UniqueCode}_{timestamp}{ext}".ToLower();

                // 3. Xây dựng đường dẫn (giữ nguyên cấu trúc phân cấp ban đầu của bạn)
                string relativeFolder = Path.Combine(dto.AppCode, dto.MediaType, dto.Entity, dto.UniqueCode);
                string physicalFolder = Path.Combine(_storageRoot, relativeFolder);

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(physicalFolder))
                    Directory.CreateDirectory(physicalFolder);

                string physicalPath = Path.Combine(physicalFolder, safeFileName);

                // 4. Lưu file vật lý
                using (var destinationStream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
                {
                    await fileStream.CopyToAsync(destinationStream);
                }

                // 5. Trả về URL tương đối (đã chuyển về lowercase cho đồng bộ)
                string relativeUrl = Path.Combine(relativeFolder, safeFileName).Replace("\\", "/").ToLower();

                return MediaResponse.Success(
                    relativeUrl: "/" + relativeUrl,
                    fullUrl: $"{_baseDomain}/{relativeUrl}",
                    size: fileStream.Length,
                    ext: ext
                );
            }
            catch (Exception ex)
            {
                return MediaResponse.Failure($"Lỗi lưu trữ: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa các tệp tin vật lý trong thư mục tương ứng với Metadata.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Hàm sử dụng Wildcard để xóa tất cả các file có UniqueCode tương ứng. 
        /// Sau khi xóa, nếu thư mục cha trống, hệ thống sẽ tự động dọn dẹp thư mục đó để giữ cho ổ đĩa sạch sẽ.
        /// </remarks>
        /// <param name="request">Thông tin định danh thư mục và tệp tin.</param>
        /// <returns>True nếu có ít nhất một file bị xóa thành công.</returns>
        public async Task<bool> DeleteMediaAsync(DeleteMediaRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 1. Chuẩn hóa metadata (BLL nắm giữ qui tắc này)
                    string appCode = request.AppCode.ToLower();
                    string mediaType = request.MediaType.ToLower();
                    string entity = request.Entity.ToLower();
                    string uniqueCode = request.UniqueCode.ToLower();

                    // 2. Xác định thư mục chứa file
                    string relativeFolder = Path.Combine(appCode, mediaType, entity, uniqueCode);
                    string physicalFolder = Path.Combine(_storageRoot, relativeFolder);

                    if (!Directory.Exists(physicalFolder)) return false;

                    // 3. Tìm file bắt đầu bằng UniqueCode (vì tên file là UniqueCode_Timestamp.ext)
                    // Cách này giúp Client không cần biết phần Timestamp đằng sau
                    var directoryInfo = new DirectoryInfo(physicalFolder);
                    var files = directoryInfo.GetFiles($"{uniqueCode}_*.*");

                    if (files != null && files.Any())
                    {
                        foreach (var file in files)
                        {
                            file.Delete();
                        }
                        // QUAN TRỌNG: Làm mới bộ nhớ đệm của directoryInfo sau khi xóa file
                        directoryInfo.Refresh();

                        // 4. Dọn dẹp thư mục nếu trống
                        if (Directory.Exists(physicalFolder) && !Directory.EnumerateFileSystemEntries(physicalFolder).Any())
                        {
                            Directory.Delete(physicalFolder);
                        }
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    // Log error
                    return false;
                }
            });
        }

        /// <summary>
        /// Xác thực quyền truy cập của ứng dụng dựa trên AppCode và ApiKey.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Cần được gọi ở Controller trước mọi tác vụ thay đổi dữ liệu (Upload/Delete).
        /// </remarks>
        /// <param name="appCode">Mã định danh ứng dụng.</param>
        /// <param name="apiKey">Mã bảo mật ứng dụng.</param>
        /// <returns>True nếu thông tin chính xác và ứng dụng đang Active.</returns>
        public async Task<bool> ValidateApiKeyAsync(string appCode, string apiKey)
        {
            if (string.IsNullOrEmpty(appCode) || string.IsNullOrEmpty(apiKey))
                return false;

            // Gọi sang lớp DAL để query DB (EF hoặc Dapper tùy bạn triển khai bên dưới)
            var app = await _appRepository.GetByAppCodeAsync(appCode);

            if (app != null && app.ApiKey == apiKey && app.IsActive)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Đăng ký một ứng dụng đối tác mới vào hệ thống.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Chỉ cần thực hiện một lần cho mỗi ứng dụng khách. 
        /// Sau khi đăng ký, hệ thống sẽ trả về ApiKey cố định để dùng cho các bước tiếp theo.
        /// </remarks>
        /// <param name="dto">Thông tin ứng dụng mới.</param>
        /// <returns>Thông tin kết quả đăng ký bao gồm mã và khóa bảo mật.</returns>
        /// <exception cref="Exception">Ném ngoại lệ nếu AppCode đã tồn tại hoặc có lỗi lưu trữ.</exception>
        public async Task<RegisterResponseDto> RegisterNewAppAsync(RegisterAppDto dto)
        {
            try
            {
                // 1. Kiểm tra AppCode đã tồn tại chưa
                if (await _appRepository.IsAppCodeExistAsync(dto.AppCode))
                    throw new Exception("AppCode này đã được sử dụng.");

                // 2. Sinh ApiKey ngẫu nhiên (sử dụng Guid hoặc RNGCryptoServiceProvider)
                string newApiKey = Guid.NewGuid().ToString("N").ToUpper(); // Ví dụ: 32 ký tự không gạch ngang

                // 3. Sử dụng Utility Mapster đã đóng gói để chuyển đổi
                // Ở đây ta tạo Model cho DAL
                var appModel = new AppPartner
                {
                    AppName = dto.AppName,
                    AppCode = dto.AppCode.ToUpper(),
                    ApiKey = newApiKey,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // 4. Lưu xuống DB qua DAL
                bool isSaved = await _appRepository.InsertAppAsync(appModel);

                if (!isSaved) throw new Exception("Không thể lưu thông tin đăng ký.");

                // 5. Trả về thông tin cho Client
                return new RegisterResponseDto
                {
                    AppCode = appModel.AppCode,
                    ApiKey = appModel.ApiKey,
                    CreatedDate = appModel.CreatedDate
                };
            }
            catch (Exception ex)
            {
                // Log lỗi tại đây nếu cần
                throw new Exception($"Đăng ký ứng dụng thất bại: {ex.Message}");
            }
        }
    }
}
