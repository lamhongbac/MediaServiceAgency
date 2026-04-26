namespace MSA.BLL.DTOs
{
    public class RegisterAppDto
    {
        public string AppName { get; set; }
        public string AppCode { get; set; }
    }

    // Cấu trúc phản hồi sau đăng ký
    public class RegisterResponseDto
    {
        public string AppCode { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
