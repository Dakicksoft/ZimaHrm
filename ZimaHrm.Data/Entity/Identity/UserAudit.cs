﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Data.Entity
{
    public class UserAudit
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public string UserId { get; private set; }

        [Required]
        public DateTimeOffset Timestamp { get; private set; } = DateTime.UtcNow;

        [Required]
        public UserAuditEventType AuditEvent { get; set; }

        public string IpAddress { get; private set; }

        public static UserAudit CreateAuditEvent(string userId, UserAuditEventType auditEventType, string ipAddress)
        {
            return new UserAudit { UserId = userId, AuditEvent = auditEventType, IpAddress = ipAddress };
        }
    }

    public enum UserAuditEventType
    {
        Login = 1,
        FailedLogin = 2,
        LogOut = 3
    }
}