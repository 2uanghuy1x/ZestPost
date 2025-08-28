//using System.Drawing;
//namespace ZestPost.Base.Helper
//{
//    public class HelperTransTextToImg
//    {
//        public int maxWidth;
//        public int maxHeight;
//        static Color GenerateRandomColor(int type)
//        {
//            Random rand = new Random();
//            int red = 0;
//            int green = 0;
//            int blue = 0;
//            switch (type)
//            {
//                case 1:
//                    red = rand.Next(150, 256);
//                    green = rand.Next(150, 256);
//                    blue = rand.Next(0, 256);
//                    break;
//                case 2:
//                    red = rand.Next(150, 256);
//                    green = rand.Next(0, 256);
//                    blue = rand.Next(150, 256);
//                    break;

//            }
//            Color color = Color.FromArgb(red, green, blue);
//            return color;
//        }

//        public void TransTextToImg(int typeText, string text, string path_file, Font font, ImageFormat imageFormat)
//        {
//            try
//            {
//                Bitmap bitmap = new Bitmap(1200, 900);
//                maxWidth = 1200;
//                maxHeight = 900;
//                using (Graphics graphics = Graphics.FromImage(bitmap))
//                {
//                    // Vẽ hình chữ nhật với màu nền xanh
//                    Color color1 = GenerateRandomColor(1);
//                    Color color2 = GenerateRandomColor(2);
//                    // Tạo một LinearGradientBrush từ điểm trên cùng đến điểm dưới cùng của hình ảnh
//                    LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, bitmap.Height), color1, color2);

//                    // Vẽ hình chữ nhật dùng LinearGradientBrush đã tạo
//                    graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
//                    SolidBrush textBrush = new SolidBrush(Color.Black); // Màu văn bản

//                    // Tạo một RectangleF để chứa văn bản trên hình ảnh
//                    SizeF textSize = Graphics.FromImage(bitmap).MeasureString(text, font);
//                    //float rectWidth = Math.Min(maxWidth, textSize.Width);
//                    float rectHeight = Math.Min(maxHeight, textSize.Height) + 100;
//                    RectangleF rect = new RectangleF(0, 0, 1200, rectHeight);

//                    // Thiết lập các thuộc tính văn bản
//                    StringFormat format = new StringFormat();
//                    switch (typeText)
//                    {
//                        case 0: // Căn chỉnh giữa
//                            format.Alignment = StringAlignment.Center; // Căn văn bản ở giữa theo chiều ngang
//                            format.LineAlignment = StringAlignment.Center; // Căn văn bản ở giữa theo chiều dọc;
//                            break;
//                        case 1: // Căn chỉnh trái
//                            format.Alignment = StringAlignment.Near; // Căn văn bản ở bên trái
//                            format.LineAlignment = StringAlignment.Center; // Căn văn bản ở giữa theo chiều dọc
//                            break;
//                        case 2: // Căn chỉnh phải
//                            format.Alignment = StringAlignment.Far; // Căn văn bản ở bên phải
//                            format.LineAlignment = StringAlignment.Center; // Căn văn bản ở giữa theo chiều dọc
//                            break;
//                    }
//                    // Vẽ văn bản lên hình ảnh
//                    graphics.DrawString(text, font, textBrush, rect, format);

//                }
//                bitmap.Save(path_file, imageFormat);
//            }
//            catch (Exception ex)
//            {
//                Log4NetSyncController.LogException(ex, "");
//            }
//        }

//        public void DeleteImg(string path_file)
//        {
//            try
//            {
//                if (File.Exists(path_file))
//                {
//                    File.Delete(path_file);
//                }
//            }
//            catch (Exception ex)
//            {
//                Log4NetSyncController.LogException(ex, "");
//            }
//        }
//    }
//}
