using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSA.BLL;
using MSA.BLL.DTOs;
using Shared.Library.Utilities;

namespace MSA.API.Controllers
{
    /// <summary>
    /// Controller quản lý các hoạt động liên quan đến Media (Upload, Delete) và đăng ký ứng dụng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaBusiness _mediaBusiness;

        /// <summary>
        /// Khởi tạo MediaController với các dependency cần thiết.
        /// </summary>
        /// <param name="mediaBusiness">Dịch vụ xử lý nghiệp vụ Media.</param>
        public MediaController(IMediaBusiness mediaBusiness)
        {
            _mediaBusiness = mediaBusiness;
        }

        /// <summary>
        /// Tải lên tệp tin media (hình ảnh, video, v.v.)
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Client phải thực hiện đăng ký ứng dụng qua API /register trước để lấy ApiKey. 
        /// Sau đó, truyền ApiKey vào Header 'X-Api-Key' và các thông tin định danh (AppCode, MediaType...) vào Form-Data.
        /// </remarks>
        /// <param name="apiKey">Mã bảo mật ứng dụng truyền qua Header X-Api-Key.</param>
        /// <param name="webRequest">Thông tin tệp tin và metadata định danh.</param>
        /// <returns>Kết quả tải lên bao gồm đường dẫn truy cập (URL).</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromHeader(Name = "X-Api-Key")] string apiKey, 
            [FromForm] UploadRequestDto webRequest)
        {
            // 1. Xác thực ApiKey
            if (!await _mediaBusiness.ValidateApiKeyAsync(webRequest.AppCode, apiKey))
            {
                return Unauthorized("ApiKey không hợp lệ hoặc ứng dụng đã bị khóa.");
            }

            // 2. Sử dụng Mapster Utility để chuyển đổi hình thái dữ liệu
            var bllRequest = webRequest.MapTo<UploadRequestDto, UploadMediaRequest>();

            using var stream = webRequest.File.OpenReadStream();
            var result = await _mediaBusiness.SaveMediaAsync(bllRequest, stream, webRequest.File.FileName);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>
        /// Xóa tệp tin media dựa trên thông tin Metadata.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Dùng khi Client muốn xóa file vật lý mà không cần biết chính xác tên file có timestamp. 
        /// Chỉ cần cung cấp đủ Metadata (AppCode, MediaType, Entity, UniqueCode).
        /// </remarks>
        /// <param name="apiKey">Mã bảo mật ứng dụng truyền qua Header X-Api-Key.</param>
        /// <param name="request">Metadata định danh file cần xóa.</param>
        /// <returns>Thông báo kết quả xóa.</returns>
        [HttpDelete("delete-by-metadata")]
        public async Task<IActionResult> Delete([FromHeader(Name = "X-Api-Key")] string apiKey, [FromBody] DeleteMediaRequest request)
        {
            // 1. Xác thực ApiKey
            if (!await _mediaBusiness.ValidateApiKeyAsync(request.AppCode, apiKey))
            {
                return Unauthorized("ApiKey không hợp lệ hoặc ứng dụng đã bị khóa.");
            }

            // 2. Gọi BLL xử lý xóa
            var isDeleted = await _mediaBusiness.DeleteMediaAsync(request);

            return isDeleted ? Ok(new { Message = "Xóa thành công" }) : NotFound("Không tìm thấy file hoặc lỗi khi xóa.");
        }

        /// <summary>
        /// Đăng ký một ứng dụng mới để sử dụng dịch vụ Media.
        /// </summary>
        /// <remarks>
        /// <b>Usage:</b> Đây là bước đầu tiên bắt buộc. Sau khi gọi thành công, hãy lưu trữ 'apiKey' 
        /// ở phía Client để sử dụng cho tất cả các request Upload/Delete sau này.
        /// </remarks>
        /// <param name="request">Thông tin ứng dụng (Tên, Mã định danh).</param>
        /// <returns>Thông tin đăng ký bao gồm ApiKey được cấp phát.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAppDto request)
        {
            var result = await _mediaBusiness.RegisterNewAppAsync(request);
            return Ok(result);
        }
    }
}
