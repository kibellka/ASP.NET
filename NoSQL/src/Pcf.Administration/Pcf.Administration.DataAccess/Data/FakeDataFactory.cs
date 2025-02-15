using System.Collections.Generic;
using System.Linq;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<Role> Roles { get; set; }

        public static List<Employee> Employees { get; set; }

        static FakeDataFactory()
        {
            Roles =
            [
                new Role()
                {
                    Id = "6769d295ba2b0e10036a1b6c",
                    Name = "Admin",
                    Description = "Администратор",
                },
                new Role()
                {
                    Id = "6769d295ba2b0e10036a1b6d",
                    Name = "PartnerManager",
                    Description = "Партнерский менеджер"
                }
            ];

            Employees =
            [
                new Employee()
                {
                    Id = "6769d295ba2b0e10036a1b6a",
                    Email = "owner@somemail.ru",
                    FirstName = "Иван",
                    LastName = "Сергеев",
                    RoleId = Roles.FirstOrDefault(x => x.Name == "Admin")?.Id,
                    AppliedPromocodesCount = 5
                },
                new Employee()
                {
                    Id = "6769d295ba2b0e10036a1b6b",
                    Email = "andreev@somemail.ru",
                    FirstName = "Петр",
                    LastName = "Андреев",
                    RoleId = Roles.FirstOrDefault(x => x.Name == "PartnerManager")?.Id,
                    AppliedPromocodesCount = 10
                }
            ];
        }
    }
}