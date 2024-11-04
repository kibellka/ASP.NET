using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;
using YamlDotNet.Core;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        private readonly Guid _partnerUid = new("0da65561-cf56-4942-bff2-22f50cf70d43");
        private readonly int _defaultLimit = 77;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        public Partner CreateActivePartner()
        {
            var partner = new Partner()
            {
                Id = _partnerUid,
                Name = "Рыба твоей мечты",
                IsActive = true,
                NumberIssuedPromoCodes = 7,
                PartnerLimits =
                [
                    new PartnerPromoCodeLimit()
                    {
                        Id = new("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2024, 10, 1),
                        EndDate = new DateTime(2024, 12, 31),
                        Limit = 99
                    }
                ]
            };

            return partner;
        }


        public SetPartnerPromoCodeLimitRequest CreateSetPartnerPromoCodeLimitRequest(int limit)
        {
            var request = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now, Limit = limit };

            return request;
        }

        // 1. Если партнер не найден, то также нужно выдать ошибку 404;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = _partnerUid;
            Partner partner = null;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(_defaultLimit);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, requestModel);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        // 2. Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            Fixture autoFixture = new Fixture();
            autoFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => autoFixture.Behaviors.Remove(b));
            autoFixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var partner = autoFixture.Build<Partner>()
                .With(t => t.IsActive, false)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(_defaultLimit);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, requestModel);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        // 3. Если партнеру выставляется лимит, то мы должны обнулить количество промокодов,
        // которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
        // При установке лимита нужно отключить предыдущий лимит;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActivePartnerWithActiveLimit_ReturnsZeroNumberIssuedPromoCodesAndExistedCancelDate()
        {
            // Arrange
            var partnerId = _partnerUid;
            var partner = CreateActivePartner();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(_defaultLimit);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, requestModel);

            // Assert
            var limit = partner.PartnerLimits.FirstOrDefault(x => x.CancelDate.HasValue);
            limit.CancelDate.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }
        // 3
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActivePartnerWithInactiveLimit_ReturnsNotZeroNumberIssuedPromoCodesAndNotExistedCancelDate()
        {
            // Arrange
            var partnerId = _partnerUid;
            var partner = CreateActivePartner();
            var limit = partner.PartnerLimits.First();
            limit.CancelDate = DateTime.Now.AddDays(-5);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(_defaultLimit);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, requestModel);

            // Assert
            limit.CancelDate.Should().NotBeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            partner.NumberIssuedPromoCodes.Should().NotBe(0);
        }

        // 4. При установке лимита нужно отключить предыдущий лимит;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsActive_ReturnsCalceledLimit()
        {
            // Arrange
            var partnerId = _partnerUid;
            var partner = CreateActivePartner();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(_defaultLimit);
            var limit = partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, requestModel);

            // Assert
            limit.CancelDate.Should().BeCloseTo(DateTime.Now, new TimeSpan(0,1,0));
        }

        // 5. Лимит должен быть больше 0;
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task SetPartnerPromoCodeLimitAsync_LimitIsLessOrEqualZero_ReturnsBadRequest(int limit)
        {
            // Arrange
            var partnerId = _partnerUid;
            var partner = CreateActivePartner();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestModel = CreateSetPartnerPromoCodeLimitRequest(limit);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, requestModel);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }
    }
}