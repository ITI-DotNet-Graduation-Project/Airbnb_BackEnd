using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Airbnb.Infrastructure.Repos;
using Airbnb.Application.DTOs;
using Airbnb.DATA.models; 
using global::Airbnb.Application.DTOs.Payment;
using global::Airbnb.Infrastructure.Repos.Implementation;
using Airbnb.Application.Services.Abstract;

    namespace Airbnb.Application.Services.Implementation
    {
    
        public class PaymentService : IPaymentService
    {
            private readonly PaymentRepo _paymentRepo;

            public PaymentService(PaymentRepo paymentRepo)
            {
                _paymentRepo = paymentRepo;
            }

            public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
            {
                var payments = await _paymentRepo.GetAllAsync();
                return payments.Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate
                });
            }

            public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
            {
                var payment = await _paymentRepo.GetByIdAsync(id);
                return new PaymentDTO
                {
                    Id = payment.Id,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment.PaymentDate
                };
            }

            public async Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto)
            {
                var payment = new Payment
                {
                    BookingId = paymentDto.BookingId,
                    Amount = paymentDto.Amount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    PaymentDate = DateTime.UtcNow
                };

                var createdPayment = await _paymentRepo.AddAsync(payment);

                return new PaymentDTO
                {
                    Id = createdPayment.Id,
                    BookingId = createdPayment.BookingId,
                    Amount = createdPayment.Amount,
                    PaymentMethod = createdPayment.PaymentMethod,
                    PaymentDate = createdPayment.PaymentDate
                };
            }

            public async Task<PaymentDTO> UpdatePaymentAsync(PaymentDTO paymentDto)
            {
                var payment = await _paymentRepo.GetByIdAsync(paymentDto.Id);

                payment.Amount = paymentDto.Amount;
                payment.PaymentMethod = paymentDto.PaymentMethod;

                await _paymentRepo.UpdateAsync(payment);

                return paymentDto;
            }

            public async Task DeletePaymentAsync(int id)
            {
                var payment = await _paymentRepo.GetByIdAsync(id);
                if (payment != null)
                {
                    await _paymentRepo.DeleteAsync(payment);
                }
            }
        }
    }

