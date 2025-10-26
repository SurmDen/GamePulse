﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.DTOs.User
{
    public class UserDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Guid Id { get; set; }
    }
}
