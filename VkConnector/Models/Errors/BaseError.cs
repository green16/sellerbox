﻿using Newtonsoft.Json;

namespace VkConnector.Models.Errors
{
    public class BaseError
    {
        [JsonProperty("error_code")]
        public int Code { get; set; }
        [JsonProperty("error_msg")]
        public string Message { get; set; }
    }
}
