﻿using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public
{
    public interface IPaymentNotificationService
    {
        Result<PaymentNotificationDto> Create(WalletDto walletDto);
        Result<PagedResult<PaymentNotificationDto>> GetUnreadPaymentNotifications(int page, int pageSize, long profileId);
    }
}
