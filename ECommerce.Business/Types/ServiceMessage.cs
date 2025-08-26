using System;
namespace ECommerce.Business.Types
{
    public class ServiceMessage
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ServiceMessage<T> : ServiceMessage
    {
        public T? Data { get; set; }
    }

}

