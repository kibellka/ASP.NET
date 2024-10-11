using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitInMemoryAsyncTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions;

        Guid _activePartnerUid = new("894b6e9b-eb5f-406c-aefa-8ccb35d39319");

        public SetPartnerPromoCodeLimitInMemoryAsyncTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("PatnersControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var dbContext = new DataContext(_contextOptions);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            dbContext.AddRange(FakeDataFactory.Partners);
            dbContext.SaveChanges();
        }

        // 6. Нужно убедиться, что сохранили новый лимит в базу данных(это нужно проверить Unit-тестом);
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActivePartner_ReturnsSavedLimit()
        {
            // Arrange
            using var context = new DataContext(_contextOptions);
            var repository = new EfRepository<Partner>(context);
            var controller = new PartnersController(repository);
            var requestModel = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now, Limit = 6 };

            // Act
            await controller.SetPartnerPromoCodeLimitAsync(_activePartnerUid, requestModel);

            // Assert
            var partner = await repository.GetByIdAsync(_activePartnerUid);
            partner.PartnerLimits.Where(x => x.EndDate == requestModel.EndDate && x.Limit == requestModel.Limit).Should().NotBeEmpty();
        }
    }
}
