using System;
using Microsoft.AspNetCore.Identity;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data
{
    public static class SeedData
    {
        private static PasswordHasher<User> hasher { get; set; } = new PasswordHasher<User>();

        internal static Role[] BuildApplicationRoles()
        {
            return new Role[2]
            {
              new Role
              {
                Id =new Guid("3e75fb3d-5784-425a-a192-db0fc00b44ba"),
                Name = Roles.Admin.ToString(),
                NormalizedName="ADMIN",
                IsSystemDefault=true
              },
              new Role
              {
                Id =new Guid("7c6aabe0-c91d-4f13-a0dd-e48e0f29442e"),
                Name =Roles.User.ToString(),
                NormalizedName ="USER",
                IsSystemDefault=true
              }
            };
        }

        internal static User[] BuildApplicationUsers()
        {
            return new User[]
            {
              new User
              {
                Id = new Guid("21dea694-f252-4f99-9786-94fe810a186a"),
                UserName = "admin@company.com",
                Email = "admin@company.com",
                //extended properties
                FirstName = "Admin",
                LastName = "User",
                AvatarURL = "/images/default_user.png",
                CompanyId = new Guid("ae4e21fa-57cb-4733-b971-fdd14c4c667e"),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PasswordHash=hasher.HashPassword(null,"Qwaszx123$"),
                LockoutEnabled = false,
                Status=UserStatus.AllGood,
                PhoneNumber="905552223311",
                NormalizedEmail="ADMIN@COMPANY.COM",
                NormalizedUserName="ADMIN@COMPANY.COM",
                SecurityStamp=Guid.NewGuid().ToString()
              },
              new User
              {
                Id = new Guid("9008b3b5-2cf3-4d0b-8f1c-96ad794e25e1"),
                UserName = "user@member.com",
                Email = "user@member.com",
                //extended properties
                FirstName = "Member",
                LastName = "User",
                AvatarURL = "/images/user.png",
                CompanyId = new Guid("ae4e21fa-57cb-4733-b971-fdd14c4c667e"),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PasswordHash=hasher.HashPassword(null,"Qwaszx123$"),
                LockoutEnabled = true,
                Status=UserStatus.AllGood,
                PhoneNumber="905551112233",
                NormalizedEmail="USER@MEMBER.COM",
                NormalizedUserName="USER@MEMBER.COM",
                SecurityStamp=Guid.NewGuid().ToString()
              }
            };
        }

        internal static Company[] BuildApplicationCompanies()
        {
            return new Company[]
            {
              new Company
              {
                  Address = "Tallin",
                  CompanyName ="Dakicksoft",
                  Country="Estonia",
                  CreatedBy="System",
                  CreatedUtc = DateTime.UtcNow,
                  Currency = "USD",
                  Email = "muhammet.sahin@dakicksoft.com",
                  Id = new Guid("ae4e21fa-57cb-4733-b971-fdd14c4c667e"),
                  IsDelete = false,
                  LastModifiedBy = "System",
                  LastModifiedUtc = DateTime.UtcNow,
                  Phone = "905553336655",
                  Logo ="/images/zimahrm_logo.png",
                  Subscription = Subscription.Pro,
                  SubscriptionExpireDate = DateTime.UtcNow,
                  Tax = "111111111",
                  Web ="https://www.dakicksoft.com"
              }
            };
        }

        internal static UserRole[] BuildApplicationUserRoles()
        {
            return new UserRole[]
            {
                //admin role to admin user
                new UserRole
                {
                    RoleId =new Guid("3e75fb3d-5784-425a-a192-db0fc00b44ba"),//Admin
                    UserId =new Guid("21dea694-f252-4f99-9786-94fe810a186a"),
                },
                //member role to member user
                new UserRole
                {
                    RoleId = new Guid("7c6aabe0-c91d-4f13-a0dd-e48e0f29442e"),//User
                    UserId =new Guid("9008b3b5-2cf3-4d0b-8f1c-96ad794e25e1"),
                }
            };
        }
    }
}
