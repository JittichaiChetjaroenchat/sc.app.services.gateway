namespace SC.App.Services.Gateway.Constant
{
    public class AppSettings
    {
        public class Applications
        {
            public class Gateway
            {
                public const string Name = "Applications:Gateway:Name";
            }
        }

        public class RateLimiting
        {
            public const string Default = "RateLimiting";
        }

        public static class ElasticSearch
        {
            public const string Uri = "ElasticSearch:Uri";
        }
    }
}