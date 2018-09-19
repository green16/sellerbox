namespace WebApplication1.Common
{
    public static class Logins
    {
#if DEBUG
        public const int VkApplicationId = 6691268;
        public const string VkApplicationPassword = "d2uFXkIYY9oQRmkQq71M";
        public const string SiteUrl = "http://vk.it-web-develop.ru";
        public const string CallbackServerUrl = SiteUrl + "/Callback/Vk";
        public const string CallbackServerName = "SellerboxTest";
#else
        public const int VkApplicationId = 6460242;
        public const string VkApplicationPassword = "vyCsMTR3S6Zyhz6FNGZs";
        public const string SiteUrl = "http://sellerbox.it-rkomi.ru";
        public const string CallbackServerUrl = SiteUrl + "/Callback/Vk";
        public const string CallbackServerName = "Sellerbox";
#endif
    }
}
