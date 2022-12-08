using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2_publish
{
    public class Extras
    {
        public class ServiceXPTO
        {
            public void DoAnything(CustomObject message) { }
        }

        public class CustomObject
        {
            public string Text { get; set; }
        }


        public class Serializer
        {
            public T Deserialize<T>(byte[] messageBytes) => default(T);
        }
    }
}
