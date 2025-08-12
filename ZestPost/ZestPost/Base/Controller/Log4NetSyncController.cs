using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ZestPost.Base.Controller
{
    public class Log4NetSyncController
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void ConfigureLog4Net()
        {
            // Tạo layout cho log
            var layout = new PatternLayout();
            layout.ActivateOptions();

            var rollingFileAppender = new RollingFileAppender
            {
                Name = "RollingFileAppender",
                // Đặt tên file với tiền tố và định dạng ngày
                File = $"{AppDomain.CurrentDomain.BaseDirectory}logs//error_sync_log",
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Date, // Tách file theo ngày
                StaticLogFileName = false, // Cho phép tên file thay đổi theo ngày
                Layout = layout,
                MaxSizeRollBackups = 30, // Số file backup tối đa (có thể điều chỉnh)
                MaximumFileSize = "100MB", // Kích thước file tối đa

            };

            rollingFileAppender.ActivateOptions(); // Gọi phương thức này sau khi thiết lập các thuộc tính

            // Tạo logger và cấu hình root logger
            var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetCallingAssembly());
            hierarchy.Root.AddAppender(rollingFileAppender);
            hierarchy.Root.Level = log4net.Core.Level.Debug; // Chọn mức độ ghi log
            hierarchy.Configured = true;
        }

        public static void LogException(Exception ex, string message, [CallerFilePath] string nameFile = "", [CallerMemberName] string nameFunction = "", [CallerLineNumber] int lineNumber = 0)
        {
            // Ghi log vào file
            try
            {
                var logBuilder = new StringBuilder();

                logBuilder.AppendLine($"[ERROR] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]")
                          .AppendLine($"File       : {System.IO.Path.GetFileName(nameFile)}")
                          .AppendLine($"Function   : {nameFunction}")
                          .AppendLine($"Line       : {lineNumber}")
                          .AppendLine($"Message    : {message}")
                          .AppendLine($"Detail     : {ex.Message}")
                          .AppendLine("Stack Trace:")
                          .AppendLine(ex.StackTrace)
                          .AppendLine("--------------------------------------------------")
                          .AppendLine();

                logger.Error(logBuilder.ToString());
            }
            catch { }
        }

        public static void LogInfo(string message, [CallerFilePath] string nameFile = "", [CallerMemberName] string nameFunction = "")
        {
            try
            {
                var logBuilder = new StringBuilder();
                // Ghi log vào file
                logBuilder.AppendLine($"[INFO] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]")
                         .AppendLine($"File       : {System.IO.Path.GetFileName(nameFile)}")
                         .AppendLine($"Function   : {nameFunction}")
                         .AppendLine($"Message    : {message}")
                         .AppendLine("--------------------------------------------------")
                         .AppendLine();
                logger.Info(logBuilder.ToString());
            }
            catch { }
        }

        public static void LogDebug(string message, [CallerFilePath] string nameFile = "", [CallerMemberName] string nameFunction = "")
        {
            try
            {
                var logBuilder = new StringBuilder();
                // Ghi log vào file
                logBuilder.AppendLine($"[DEBUG] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]")
                          .AppendLine($"File       : {System.IO.Path.GetFileName(nameFile)}")
                          .AppendLine($"Function   : {nameFunction}")
                          .AppendLine($"Message    : {message}")
                          .AppendLine("--------------------------------------------------")
                          .AppendLine();
                logger.Info(logBuilder.ToString());
            }
            catch { }
        }
    }
}
