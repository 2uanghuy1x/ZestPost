namespace ZestPost.Base
{
    public static class ObjectExtension
    {
        public static bool IsEmpty(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotEmpty(this object obj)
        {
            return !obj.IsEmpty();
        }

        public static string AsString(this object obj)
        {
            return obj.IsNotEmpty() ? obj.ToString() : "";
        }
    }
}
