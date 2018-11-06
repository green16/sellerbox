using System;
using Xunit;

namespace SellerBox.Tests.Common.Helpers
{
    public class VkHelper_CanBirthdayConvert
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ReturnNullOnEmptyString(string birthday)
        {
            var result = SellerBox.Common.Helpers.VkHelper.BirtdayConvert(birthday);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("12102018")]
        [InlineData("12.102018")]
        [InlineData("32.13")]
        public void ReturnNullOnInvalidString(string birthday)
        {
            var result = SellerBox.Common.Helpers.VkHelper.BirtdayConvert(birthday);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("12.10.2018")]
        [InlineData("12.10")]
        public void ReturnDateOnCorrectString(string birthday)
        {
            var result = SellerBox.Common.Helpers.VkHelper.BirtdayConvert(birthday);
            Assert.NotNull(result);
            Assert.IsType<DateTime>(result);
            Assert.Equal(new DateTime(2018, 10, 12), result);
        }
    }
}
