# GUI & API Interface Standards

Quy chuẩn phát triển tầng giao diện và điểm tiếp nhận yêu cầu (API), tập trung vào trải nghiệm người dùng và tính hợp lệ của dữ liệu.

## 1. Cấu trúc Solution
- **{Project}.API**: Tiếp nhận request và điều phối.
- **{Project}.GUI (Web/Mobile)**: Dự án chuyên biệt về giao diện.
    - Kiểu dữ liệu: **ViewModels** (Chuyên biệt cho hiển thị và nhận input).
    - Có tầng **GUI Services** để gọi sang BLL.

## 2. ViewModel & Validation

### A. DataAnnotations
Sử dụng các thuộc tính `[Required]`, `[StringLength]`, `[EmailAddress]`... trên **ViewModel** để kiểm tra dữ liệu ngay tại tầng giao diện.

### B. Custom Validation
Đối với logic kiểm tra phức tạp, triển khai các Attribute tùy chỉnh:
```csharp
public class BusinessAuthorizationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Logic kiểm tra đặc thù
        return ValidationResult.Success;
    }
}
```

## 3. Xử lý tại Controller/Page
- Luôn kiểm tra `ModelState.IsValid` trước khi gọi xuống tầng nghiệp vụ.
- Tuyệt đối không dùng trực tiếp DAL Model lên View.

## 4. Thành phần Giao diện (Blazor/Razor)
Đảm bảo hiển thị lỗi tường minh:
- Sử dụng `<DataAnnotationsValidator />`.
- Sử dụng `<ValidationSummary />` cho lỗi tổng hợp.
- Sử dụng `<ValidationMessage For="..." />` cho từng trường.
```html
<div class="form-group">
    <label>Tên đăng nhập:</label>
    <InputText @bind-Value="loginModel.Username" />
    <ValidationMessage For="@(() => loginModel.Username)" />
</div>
```
