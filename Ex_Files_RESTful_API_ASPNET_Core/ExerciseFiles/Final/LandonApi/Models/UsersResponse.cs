using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class UsersResponse : PagedCollection<User>
    {
        public Form Register { get; set; }

        public Link Me { get; set; }
    }
}
