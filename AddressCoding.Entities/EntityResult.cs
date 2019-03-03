using System;
using System.Collections.Generic;

namespace AddressCoding.Entities
{
    public class EntityResult<T>
    {
        public bool Result { get; set; }
        public Exception Error { get; set; }
        public T Object { get; set; }
        public  IEnumerable<T> Objects { get; set; }
    }
}