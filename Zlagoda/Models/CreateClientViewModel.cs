﻿using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CreateClientViewModel
    {
        public string Title { get; set; } = string.Empty;
        public CustomerCard Client { get; set; } = new CustomerCard();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
