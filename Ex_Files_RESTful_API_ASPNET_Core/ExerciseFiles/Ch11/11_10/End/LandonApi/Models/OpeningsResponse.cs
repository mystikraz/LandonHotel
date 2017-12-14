using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class OpeningsResponse : PagedCollection<Opening>
    {
        public Form OpeningsQuery { get; set; }
    }
}
