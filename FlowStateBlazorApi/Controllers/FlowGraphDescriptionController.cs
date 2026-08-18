using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlowStateBlazorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowGraphDescriptionController : DefaultControllerT<FlowGraphDescription>
    {
        public FlowGraphDescriptionController(FlowStateContext context) : base(context)
        {
        }
    }
}
