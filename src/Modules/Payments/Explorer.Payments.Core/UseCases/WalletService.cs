﻿using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletService: CrudService<WalletDto, Wallet>, IWalletService
    {
        private readonly ICrudRepository<Wallet> _walletRepository;
        private readonly IPaymentNotificationService _paymentNotificationsService;
        private readonly IWalletRepository __walletRepository;
        public WalletService(ICrudRepository<Wallet> crudRepository, IMapper mapper, ICrudRepository<Wallet> walletRepository, IPaymentNotificationService paymentNotificationsService, IWalletRepository wallet) : base(crudRepository, mapper)
        {
            _walletRepository = walletRepository;
            _paymentNotificationsService = paymentNotificationsService;
            __walletRepository = wallet;
        }


        public Result<WalletDto> AddAC(WalletDto walletDto)
        {
            Wallet wallet = _walletRepository.Get(walletDto.Id);
            if (wallet != null)
            {
                wallet.AC += walletDto.AC;
                _walletRepository.Update(wallet);
                _paymentNotificationsService.Create(walletDto);
                return MapToDto(wallet);
            }
            return Result.Fail(FailureCode.NotFound).WithError("Wallet not found.");
        }

        public Result<WalletDto> GetWalletByUserId(int userId)
        {
            Wallet wallet = __walletRepository.GetWalletByUserId(userId);
            return MapToDto(wallet);
        }

      

        public Result<int> SendAC(int AC, int receiverWalletId, int senderWalletId)
        {
            Wallet receiverWallet = _walletRepository.Get(receiverWalletId);
            Wallet senderWallet = _walletRepository.Get(senderWalletId);
            if (receiverWallet != null && senderWallet != null)
            {  
                    receiverWallet.AC += AC;
                    senderWallet.AC -= AC;
                    _walletRepository.Update(receiverWallet);
                    _walletRepository.Update(senderWallet);
                    return AC;
               
            }
            return Result.Fail(FailureCode.NotFound).WithError("Wallet not found.");
        }

       


    }
}
