using System;
using System.Collections.Generic;
using System.Text;

namespace FlowStateBlazor.Data.Models
{
    public interface IParentT<T> where T : class, IIdAndNamed
    {
        public int ParentId { get; set; }
        public T? Parent { get; set; }
    }
}
