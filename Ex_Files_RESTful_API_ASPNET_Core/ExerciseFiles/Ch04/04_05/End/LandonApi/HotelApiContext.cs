using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi
{
    public class HotelApiContext : DbContext
    {
        public HotelApiContext(DbContextOptions options)
            : base(options) { }

        public DbSet<RoomEntity> Rooms { get; set; }
    }
}
