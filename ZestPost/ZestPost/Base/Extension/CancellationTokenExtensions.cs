namespace ZestPost.Base.Extension
{
    public static class CancellationTokenExtensions
    {
        public static bool IsStopped(this CancellationToken token)
        {
            return token == default || token.IsCancellationRequested;
        }

        public static bool IsRunning(this CancellationToken token)
        {
            return !token.IsStopped();
        }
    }
}
