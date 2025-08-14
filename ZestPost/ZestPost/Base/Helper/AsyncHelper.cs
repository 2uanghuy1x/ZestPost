namespace ZestPost.Base.Helper
{
    public static class AsyncHelper
    {
        public static void RunSync(Func<Task> task)
        {
            Task.Run(task).GetAwaiter().GetResult();
        }
        public static T RunSync<T>(Func<Task<T>> task)
        {
            return Task.Run(task).GetAwaiter().GetResult();
        }
    }
}
