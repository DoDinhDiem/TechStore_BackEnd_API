﻿using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public partial class Slider
    {
        public int Id { get; set; }
        public string? TenSlide { get; set; }
        public string? AnhSlide { get; set; }
        public int? SapXep { get; set; }
        public string? Link { get; set; }
        public bool? Status { get; set; }
    }
}
