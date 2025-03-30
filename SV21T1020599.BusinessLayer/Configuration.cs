namespace SV21T1020599.BusinessLayers
{
    public static class Configuration
    {
        private static string connectionString = "";

        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }

        public static string Connectionstring
        {
            get
            {
                return connectionString;
            }
        }
    }
}