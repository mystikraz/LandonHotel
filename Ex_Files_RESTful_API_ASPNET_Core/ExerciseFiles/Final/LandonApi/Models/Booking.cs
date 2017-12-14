using LandonApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class Booking : Resource
    {
        public Link Room { get; set; }

        public Link User { get; set; }

        public Link Cancel { get; set; }

        [Sortable]
        [SearchableDateTime]
        public DateTimeOffset StartAt { get; set; }

        [Sortable]
        [SearchableDateTime]
        public DateTimeOffset EndAt { get; set; }

        [Sortable]
        [SearchableDateTime]
        public DateTimeOffset CreatedAt { get; set; }

        [Sortable]
        [SearchableDateTime]
        public DateTimeOffset ModifiedAt { get; set; }

        [Sortable]
        [SearchableDecimal]
        public decimal Total { get; set; }
    }
}
