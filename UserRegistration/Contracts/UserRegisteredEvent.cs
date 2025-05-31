﻿namespace Contracts
{
    public class UserRegisteredEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}
