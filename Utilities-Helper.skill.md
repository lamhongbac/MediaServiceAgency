# Utilities & Helper Standards

Quy chuẩn cho các thư viện dùng chung, hàm tiện ích và xử lý hệ thống bổ trợ trong dự án.

## 1. Cấu trúc Solution
- **{Project}.Shared.Library**: Chứa các Utility dùng chung không phụ thuộc vào layer cụ thể.

## 2. Chiến lược Ánh xạ dữ liệu (Mapping Strategy)
Để đảm bảo tính tách biệt, dự án sử dụng chiến lược ánh xạ dữ liệu nghiêm ngặt giữa các tầng.

- **Công cụ**: Sử dụng **Mapster** để thực hiện ánh xạ tự động.
- **Luồng ánh xạ**:
    - `ViewModel` <-> `BO` (Thực hiện tại **GUI Services**).
    - `BO` <-> `Model` (Thực hiện tại **BLL**).
- **Nguyên tắc**: Tuyệt đối không dùng chung kiểu dữ liệu giữa các tầng khác nhau.

## 3. Xử lý File & IO
- **Stream**: Ưu tiên xử lý qua Stream thay vì byte array để tối ưu RAM.
- **Cấu trúc lưu trữ**: Phân cấp rõ ràng: `Root/Type/Entity/UniqueCode/File`.
- **Naming**: `{UniqueCode}_{Timestamp}` và luôn chuyển về **lowercase**.

## 4. Các Hàm Tiện ích (Helpers)
- **StringHelper**: Xử lý cắt chuỗi, định dạng, remove dấu tiếng Việt.
- **DateTimeHelper**: Định dạng ngày giờ chuẩn hệ thống.
- **SecurityHelper**: Các hàm hash, mã hóa nhẹ.

## 5. Cấu hình & Constants
- Lưu trữ các chuỗi hằng số (Constants), Key cấu hình dùng chung để tránh hardcode.
