using LandonApi.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class RootResponse : Resource, IEtaggable
    {
        public Link Info { get; set; }

        public Link Rooms { get; set; }

        public Link Users { get; set; }

        public Form Token { get; set; }

        public string GetEtag()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return Md5Hash.ForString(serialized);
        }
    }
}
