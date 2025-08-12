using OpenQA.Selenium.Chrome;
using ZestPost.Base.Chrome;

namespace ZestPost.Service
{
    public class EventHandlerService
    {
        public void HandleEvent(EventData eventData)
        {
            switch (eventData.Screen)
            {
                case "accounts":
                    HandleAccountsScreen(eventData.Data);
                    break;
                case "details":
                    HandleDetailsScreen(eventData.Data);
                    break;
                case "settings":
                    HandleSettingsScreen(eventData.Data);
                    break;
                default:
                    MessageBox.Show($"Màn hình không xác định: {eventData.Screen}");
                    break;
            }
        }

        private void HandleAccountsScreen(List<Account> data)
        {
            try
            {
                testchrome testchrome = new testchrome(new ChromeDriver());
                testchrome.OpenChrome("aa");
                //MessageBox.Show($"Tài khoản được chọn: {string.Join(", ", selectedAccounts)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý tài khoản: {ex.Message}");
            }
        }

        private void HandleDetailsScreen(object data)
        {
            try
            {
                var details = JsonConvert.DeserializeObject<DetailData>(data.ToString());
                MessageBox.Show($"Chi tiết tài khoản: ID = {details.AccountId}, Tên = {details.AccountName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý chi tiết: {ex.Message}");
            }
        }

        private void HandleSettingsScreen(object data)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<SettingsData>(data.ToString());
                MessageBox.Show($"Cài đặt: Theme = {settings.Theme}, Notifications = {settings.Notifications}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý cài đặt: {ex.Message}");
            }
        }
    }

    // Định nghĩa lớp dữ liệu
    public class EventData
    {
        [JsonProperty("screen")]
        public string Screen { get; set; }

        [JsonProperty("data")]
        public List<Account> Data { get; set; }
    }

    public class Account
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class DetailData
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
    }

    public class SettingsData
    {
        public string Theme { get; set; }
        public bool Notifications { get; set; }
    }
}
