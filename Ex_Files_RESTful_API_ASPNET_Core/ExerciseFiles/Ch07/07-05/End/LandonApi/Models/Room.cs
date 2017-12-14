﻿using LandonApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class Room : Resource
    {
        [Sortable]
        public string Name { get; set; }

        [Sortable(Default = true)]
        public decimal Rate { get; set; }
    }
}
