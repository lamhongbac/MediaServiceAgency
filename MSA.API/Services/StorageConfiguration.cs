using MSA.BLL;

namespace MSA.API.Services
{
    public class StorageConfiguration : IStorageConfiguration
    {
        
        private readonly IWebHostEnvironment _env;
        public StorageConfiguration(IWebHostEnvironment env) => _env = env;
        public string GetRootPath() => _env.WebRootPath;
    }
}
