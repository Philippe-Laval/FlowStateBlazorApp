using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowStateBlazor.Data.Services
{
    public class FlowGraphDescriptionService : DefaultServiceTWithIdAndName<FlowGraphDescription>
    {
        public FlowGraphDescriptionService(FlowStateContext dbContext) : base(dbContext)
        {
        }
    }
}
