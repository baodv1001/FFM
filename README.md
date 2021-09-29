# Quản lí sân bóng mini 

 

## Mô tả 

 

Hiện nay, ở các nơi cho thuê sân bóng đá, việc đặt sân, quản lý giờ trống còn gặp nhiều khó khăn, bất tiện và tốn chi phí điện thoại. Phần mềm quản lý sân bóng đá sẽ giúp cho các chủ sân có thể quản lý dễ dàng, đơn giản hơn và người có nhu cầu đặt sân cũng dễ dàng biết được giờ trống để đặt sân.Bên cạnh đó là việc quản lý nhân viên, quản lý dịch vụ và quản lý doanh thu một cách bảo mật, chi tiết. 

### Người dùng 

* Chủ sở hữu sân bóng 

* Nhân viên quản lý 

* Nhân viên thu ngân 

### Mục tiêu 

#### Ứng dụng thực tế 

* Chủ sở hữu sân bóng: quản lý doanh thu, nhân viên, tình trạng cơ sở vật chất, phản hồi của khách 

* Nhân viên quản lý: giúp nhân viên quản lý dễ dàng, không bị xung đột hay nhầm lẫn , quản lý dịch vụ khác, quản lý được tình trạng thiết bị vật dụng 

* Nhân viên thu ngân: Thực hiện việc kinh doanh 

#### Yêu cầu ứng dụng 

* Giao diện hợp lý, rõ ràng và thân thiện với người dùng. 

* Quản lý được lịch trống, tình trạng sân bóng, các mặt hàng phụ trợ và phản hồi của khách hàng. 

* Dễ dàng sử dụng. 

### Tính năng 

* Đăng nhập 

* Tạo tài khoản nhân viên, đổi mật khẩu 

* Quản lý sân bóng 

  * Thêm, xóa, sửa 

  * Đặt sân, hủy sân 

  * Check in, check out 

  * Thanh toán sân 

  * Xuất hóa đơn 

  * Báo cáo tình trạng sân 

  * Kiểm tra trạng thái sân 

* Quản lý nhân viên 

  * Thêm, xóa, sửa 

  * Tính lương 

  * Chấm công 

* Quản lý dịch vụ 

  * Thêm, xóa, sửa hàng hóa 

  * Nhập hàng 

  * Xuất hóa đơn 

  * Quản lý hàng tồn kho 

* Quản lý tài khoản, thông tin sân 

  * Đổi thông tin sân 

  * Đổi mật khẩu cho tài khoản 

* Thống kê thu chi 

  * Thống kê doanh thu 

  * Xem báo cáo kinh doanh, nhập kho, trả lương 

### Công nghệ 

* Ngôn ngữ: C#  nền tảng .Net FrameWork v4.7.2 

* IDE: Visual Studio 2019  

* UI Framework: Windows Presentation Foundation (WPF) 

* Database: SQL Server 

* UI design: Figma, Adobe Illustrator 

* Công cụ quản lý sourcecode: GitHub 

* Công cụ quản lý quá trình làm việc: Trello 

* Khác: Office365, OneDrive, Github Desktop, Microsoft Teams, Facebook 

## Tác giả 

* [Đỗ Văn Bảo](https://www.facebook.com/ghostlove1001) - 19521238 - Team leader, Developer 

* [Huỳnh Quang Trung](https://www.facebook.com/hqt234) - 19520317 - UI/UX designer, Developer 

* [Phan Ngọc Quang](https://www.facebook.com/quangs.pn) - 19522100 - Developer 

 * Sinh viên khoa Công nghệ Phần mềm, trường Đại học Công nghệ Thông tin, Đại học Quốc gia thành phố Hồ Chí Minh 

## Giảng viên hướng dẫn 

* Thầy Nguyễn Tấn Toàn, giảng viên Khoa Công Nghệ Phần Mềm, trường Đại học Công nghệ Thông tin, Đại học Quốc gia Thành phố Hồ Chí Minh 

## Hướng dẫn cài đặt 

### Với người sử dụng 

* Download và giải nén phần mềm tại đường dẫn: https://tinyurl.com/FFM2021

* Cài đặt SQL Server và khởi tạo Database bằng cách query script chứa trong file Database.sql ở server

* Giải nén và chạy file Setup.msi hoặc Setup.exe 

* Kết nối với server

* Cách kết nối client pc với server trong mạng LAN 

  * Lấy IP của server pc bằng cách mở Command Prompt và gõ ipconfig 

  * Tìm mục Wireless LAN adapter Local Area Connection* 2 

  * Lấy địa chỉ của IPv4 adress 

  * Sau đó mở file FootballFieldManagement.config.exe ở server pc và sửa connectionString="
  Server = {0},{1};Initial Catalog = FootballFieldManagement;User ID = {2};Password = {3};Integrated Security = False;Connect Timeout = 20;"
(trong đó: {0} là địa chỉ IP của server, {1} port kết nối, {2} id tài khoản server, {3} mật khẩu tài khoản)
  * Lưu thông tin 

* Đăng nhập vào hệ thống với địa chỉ Chủ sân với tên đăng nhập là: admin và mật khẩu là: 1 

### Với nhà phát triển 

* Download và giải nén phền mềm tại Github: https://github.com/ghostlove1001/FootballFieldManagement hoặc tại đường dẫn: https://tinyurl.com/FFM2021

* Cài đặt SQL Server và khởi tạo Database bằng cách query script chứa trong file Database.sql (Có thể mở bằng word, notepad)

* Mở file FootballFieldManagement.sln và kết nối phần mềm với Database vừa tạo   

## Phản hồi 

Tạo phản hồi ở mục Issues, mỗi phản hồi của bạn sẽ giúp chúng tôi cải thiện ứng dụng tốt hơn. Cảm ơn vì sự giúp đỡ! 
