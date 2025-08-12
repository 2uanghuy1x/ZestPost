using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ZestPost.Base.Chrome
{
    public class testchrome
    {
        private IWebDriver Driver { get; set; }
        public testchrome(IWebDriver driver)
        {
            Driver = driver;

        }

        public void OpenChrome(string name)
        {
            try
            {
                string data = AppDomain.CurrentDomain.BaseDirectory;
                // Đường dẫn đến chromedriver.exe
                string chromeDriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver");
                // Đường dẫn đến thư mục profile
                string userDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChromeProfile");
                // Tạo thư mục profile nếu chưa tồn tại
                if (!Directory.Exists(userDataDir))
                {
                    Directory.CreateDirectory(userDataDir);
                }

                // Cấu hình ChromeOptions
                ChromeOptions options = new ChromeOptions();
                options.AddArgument($"user-data-dir={userDataDir}"); // Sử dụng thư mục profile
                options.AddArgument("--profile-directory=Default");  // Profile mặc định
                // Đảm bảo không chạy ẩn danh
                options.AddExcludedArgument("incognito");

                // Khởi tạo ChromeDriver
                Driver = new ChromeDriver(chromeDriverPath, options);

                // Mở trang web
                Driver.Navigate().GoToUrl("https://www.google.com");

                MessageBox.Show("Chrome đã mở với profile thông thường!", "Thành công");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

    }

}
