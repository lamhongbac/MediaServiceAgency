# Project Development Guidelines: Clean Architecture & Dapper Pattern

Dưới đây là bộ khung chỉ dẫn (Skill) được đúc kết từ dự án MediaServiceAgency (MSA), tập trung vào hiệu năng cao, mã nguồn sạch và khả năng mở rộng.

## 1. Cấu trúc Solution (3-Layer Clean Architecture)
Luôn chia dự án thành 4 Project chính để đảm bảo tính tách biệt (Separation of Concerns):
- **{Project}.API**: Chứa Controllers, Middleware, cấu hình `Program.cs`. Chỉ làm nhiệm vụ tiếp nhận request và điều phối.
- **{Project}.BLL (Business Logic Layer)**: Chứa logic nghiệp vụ, quy tắc đặt tên, điều phối luồng dữ liệu (Stream). Không truy cập trực tiếp DB.
- **{Project}.DAL (Data Access Layer)**: Chứa Models (Entity), Repositories. Sử dụng **Dapper** để tối ưu hiệu năng truy vấn.
- **{Project}.Shared.Library**: Chứa các Utility dùng chung như Mapper, Helper, Constant.

## 2. Tiêu chuẩn Coding (Coding Style)

### A. Chiến lược Data Access (DAL Strategy)
Tùy thuộc vào quy mô dự án, lựa chọn công nghệ truy cập dữ liệu phù hợp:

1. **Dapper (Hiệu năng cao / Dự án nhỏ / Query phức tạp)**:
    - Sử dụng khi cần kiểm soát SQL tuyệt đối và tối ưu tốc độ.
    - Luôn sử dụng `IDbConnection` thông qua Dependency Injection.
    - Sử dụng Parameterized Query (`@Param`) để chống SQL Injection.
    - Tận dụng `ExecuteScalarAsync`, `QueryFirstOrDefaultAsync`, `ExecuteAsync`.

2. **Entity Framework Core (Dự án quy mô lớn / Nhiều bảng / CRUD nhiều)**:
    - **Code-First**: Sử dụng khi muốn làm chủ cấu hình DB qua mã nguồn C# (Migrations).
    - **DB-First**: Ưu tiên sử dụng **EF Core Power Tools** để đồng bộ (Reverse Engineer) các thay đổi từ Database (Bảng, View, Stored Procedure) về Models trong dự án một cách nhanh chóng và chính xác.
    - **Tối ưu**: Luôn sử dụng `.AsNoTracking()` cho các truy vấn chỉ đọc để tăng hiệu năng.

3. **Repository Pattern**:
    - Dù dùng Dapper hay EF Core, luôn bọc logic truy cập dữ liệu trong lớp **Repository**.
    - Việc này giúp tầng BLL không bị phụ thuộc vào công nghệ DB cụ thể, dễ dàng thay đổi ORM hoặc Unit Test.

### B. Ánh xạ dữ liệu (Mapping) với Mapster
- Không map thủ công. Sử dụng **Mapster** thông qua một `MapperService` tĩnh với các Extension Methods:
  ```csharp
  public static TDestination MapTo<TSource, TDestination>(this TSource source);
  ```
- Tách biệt hoàn toàn giữa **Entity (DAL)** và **DTO (BLL/API)**.

### C. Xử lý File & IO
- Luôn ưu tiên xử lý qua **Stream** thay vì byte array hoặc tải toàn bộ vào RAM.
- **Cấu trúc lưu trữ vật lý**: Phân cấp thư mục rõ ràng (ví dụ: `App/Type/Entity/UniqueCode/File`).
- **Naming Convention**: Tên file nên bao gồm `{UniqueCode}_{Timestamp}` và chuyển về **lowercase**.

### D. Dependency Injection (DI)
- Đăng ký Services theo đúng Scope:
  - `IDbConnection`: `AddScoped` (mỗi request một connection).
  - Repository/Business: `AddScoped`.

## 3. Bảo mật & Hiệu năng API
- **Authentication**: Sử dụng `X-Api-Key` truyền qua Header thay vì Query String.
- **Static Files (CDN Nội bộ)**:
  - Map thư mục vật lý ra Virtual Path (`/cdn`).
  - Thêm Header `X-Content-Type-Options: nosniff`.
  - Cấu hình Cache trình duyệt (`max-age: 31536000`).
- **Error Handling**: BLL trả về `ResponseDto` chứa `IsSuccess` và `Message` thay vì ném Exception ra API.

## 4. Quy trình Kiểm thử (Testing Strategy)
- **Unit Test**: Sử dụng **XUnit** để kiểm tra logic ánh xạ (Mapper) và các hàm xử lý chuỗi/logic trong BLL.
- **Integration Test**: Sử dụng **PowerShell Script** để giả lập các kịch bản thực tế (Upload/Delete) gọi trực tiếp vào API.

## 5. Tài liệu hóa (Documentation)
- **SQL Scripts**: Luôn có tệp `.sql` đi kèm để khởi tạo schema database.
- **XML Comments chuyên sâu**: 
  - Sử dụng thẻ `<summary>` để tóm tắt chức năng.
  - Sử dụng thẻ `<remarks>` để bổ sung phần **Usage** (Cách dùng và Khi nào dùng).
  - Đối với API: Phải chỉ rõ thứ tự thực hiện (ví dụ: đăng ký app trước khi upload).
  - Đối với Cấu hình: Giải thích ý nghĩa các giá trị trong `appsettings.json` và thời điểm cần setup/thay đổi.
  - Đối với Logic đặc biệt: Giải thích các cơ chế tự động (ví dụ: tự dọn dẹp thư mục trống).

## 6. Quy tắc Lập trình Chuyên nghiệp (Professionalism & Standards)
Để đảm bảo dự án đạt chuẩn công nghiệp và dễ dàng bảo trì, mọi lập trình viên cần tuân thủ:

### A. Quản lý Mã nguồn (Git Standards)
- **Conventional Commits**: Sử dụng tiền tố để phân loại commit:
    - `feat:` : Thêm tính năng mới.
    - `fix:` : Sửa lỗi.
    - `docs:` : Cập nhật tài liệu.
    - `refactor:` : Tối ưu code nhưng không thay đổi tính năng.
    - `perf:` : Tối ưu hiệu năng.
- **Atomic Commits**: Mỗi commit chỉ nên giải quyết một vấn đề duy nhất. Không dồn quá nhiều thay đổi vào một commit.

### B. Quy chuẩn Đặt tên (Naming Conventions)
- **C# Standard**:
    - `PascalCase` cho Tên lớp, Interface (`I...`), Method, Public Property.
    - `camelCase` cho biến cục bộ, tham số hàm.
    - `_camelCase` (có gạch dưới) cho các private fields.
- **Súc tích & Ý nghĩa**: Tránh đặt tên biến kiểu `a`, `b`, `temp`. Tên phải nói lên mục đích (vd: `isUserAuthenticated` thay vì `check`).

### C. Vệ sinh Mã nguồn (Code Hygiene)
- **DRY (Don't Repeat Yourself)**: Nếu một đoạn code xuất hiện quá 2 lần, hãy tách nó thành hàm hoặc helper dùng chung.
- **No Commented Code**: Không commit code đã bị comment (rác). Nếu không dùng, hãy xóa nó đi (Git đã lưu lịch sử).
- **Hardcode Prevention**: Tuyệt đối không hardcode chuỗi kết nối, API Key hay đường dẫn vật lý. Tất cả phải nằm trong `appsettings.json` hoặc Environment Variables.

### D. Xử lý Lỗi & Log (Error Handling & Logging)
- **Fail Fast**: Kiểm tra dữ liệu đầu vào (Validation) ngay lập tức. Nếu sai, trả về lỗi ngay thay vì để nó chạy sâu vào logic.
- **Graceful Error**: Luôn trả về thông báo lỗi thân thiện cho client qua `ResponseDto`. Chỉ ghi log chi tiết lỗi hệ thống (StackTrace) vào server, không gửi cho client vì lý do bảo mật.

### E. Tư duy Sản phẩm (Product Mindset)
- **Sẵn sàng cho CI/CD**: Mã nguồn phải luôn ở trạng thái "Build & Run" được ngay sau khi clone.
- **README chất lượng**: Mỗi dự án phải có file `README.md` hướng dẫn Setup môi trường trong tối đa 5 phút.
