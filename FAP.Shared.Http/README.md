## FAP.Shared.Http

`FAP.Shared.Http` là thư viện **hạ tầng HTTP dùng chung** cho các ứng dụng trong hệ thống FAP.  
Mục tiêu là gom toàn bộ logic gọi API, gắn JWT, refresh token và cấu hình `HttpClient` về một chỗ, để các dự án khác có thể **dùng lại** mà không cần copy code.

### Chức năng chính

- **Chuẩn hóa cách gọi API HTTP**
  - Cung cấp `ApiClientBase` (base class) với các helper như:
    - `GetAsync<TResponse>(...)`
    - `PostAsync<TRequest, TResponse>(...)`
    - `PostAsync<TRequest>(...)`
  - Xử lý chung việc:
    - Thiết lập `HttpClient`
    - Deserialize JSON
    - Xử lý lỗi HTTP cơ bản

- **Quản lý JWT / Token tập trung**
  - Interface `ITokenStore` cho phép ứng dụng bên ngoài định nghĩa nơi lưu token (ví dụ: Session, Cache, DB…).
  - `JwtDelegatingHandler`:
    - Tự động gắn header `Authorization: Bearer <token>` cho mỗi request.
    - Hỗ trợ cơ chế refresh token (thông qua `IAuthApi`).
  - `IAuthApi` + `AuthApiClient`:
    - Chuẩn hóa việc gọi API refresh token (`/api/auth/refresh-token`).

- **Cấu hình DI (Dependency Injection) cho HttpClient**
  - `ServiceCollectionExtensions` cung cấp các extension:
    - `AddSharedHttpClients(string baseAddress)`:
      - Đăng ký `JwtDelegatingHandler`.
      - Đăng ký `IAuthApi` + `AuthApiClient` với `BaseAddress` và header JSON mặc định.
    - `AddAuthenticatedHttpClient<TClient, TImplementation>(string baseAddress)`:
      - Đăng ký typed client có:
        - `BaseAddress` chuẩn.
        - Header `Accept: application/json`.
        - Tự động gắn `JwtDelegatingHandler`.

### Cách sử dụng cơ bản

Trong `Program.cs` (hoặc nơi cấu hình DI):

```csharp
using FAP.Shared.Http.Extensions;

var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"];

// Đăng ký hạ tầng HTTP dùng chung
builder.Services.AddSharedHttpClients(apiBaseAddress);

// Đăng ký một API client nghiệp vụ sử dụng JWT
builder.Services.AddAuthenticatedHttpClient<IMyApiClient, MyApiClient>(apiBaseAddress);
```

- Ứng dụng bên ngoài chịu trách nhiệm:
  - Triển khai `ITokenStore` (ví dụ: `SessionTokenStore` trong `FAP.Admin.Web`).
  - Tạo các interface/implementation client nghiệp vụ (UserManagement, Permission, …) kế thừa `ApiClientBase` hoặc implement `IApiClient`.

### Lợi ích

- Đồng nhất cách gọi API và xử lý JWT giữa các dự án.
- Giảm trùng lặp code HttpClient / header / deserialize.
- Dễ bảo trì, dễ mở rộng cho các ứng dụng mới (chỉ cần tham chiếu `FAP.Shared.Http` và cấu hình DI).


