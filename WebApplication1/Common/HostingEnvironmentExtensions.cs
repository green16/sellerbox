using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace WebApplication1.Common
{
    public static class HostingEnvironmentExtensions
    {
        public static void UseRootNodeModules(this IHostingEnvironment hostingEnvironment)
        {
            var nodeDir = Path.Combine(hostingEnvironment.ContentRootPath, "../node_modules");
            Environment.SetEnvironmentVariable("NODE_PATH", nodeDir);
        }
    }
}
