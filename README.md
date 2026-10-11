ก่อนเริ่ม ตรวจสอบให้แน่ใจว่ามีสิ่งเหล่านี้ติดตั้งอยู่แล้ว

- **.NET SDK 10**
  👉 https://dotnet.microsoft.com/download
- **Visual Studio 2022 หรือใหม่กว่า**
  (แนะนำให้ติดตั้ง workload: ASP.NET and web development)

ตรวจสอบเวอร์ชัน:
```bash
dotnet --version
git --version


วิธีติดตั้งและรันโปรเจกต์
1. Clone Repository

2.เข้าโฟลเดอร์โปรเจกต์

3.Restore Dependencies
  dotnet restore

4. ตั้งค่า Database
 แก้ไขไฟล์ appsettings.json

 "ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=testdb;User=root;Password=yourpassword;"
}

5. Run Migration (ต้องเปิด SQL ก่อน)
  dotnet ef database update

6.รันโปรเจกต์
  dotnet run