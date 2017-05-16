using System.Collections.Generic;
using TMS.Common.Serialization.Json;

namespace Osge.Axion.State.JsonData
{
    [JsonDataContract]
    public class BaseResponseData<T>
    {
        [JsonDataMember("e")]
        public int ErrorCode { get; set; }

        [JsonDataMember("r")]
        public T Response { get; set; }
    }

    [JsonDataContract]
    public class BaseResponseArrayData<T>
    {
        [JsonDataMember("e")]
        public int ErrorCode { get; set; }

        [JsonDataMember("r")]
        public T[] Response { get; set; }
    }

    [JsonDataContract]
    public class BaseResponseDictionaryData<TK,TV>
    {
        [JsonDataMember("e")]
        public int ErrorCode { get; set; }

        [JsonDataMember("r")]
        public Dictionary<TK, TV> Response { get; set; }
    }
}
