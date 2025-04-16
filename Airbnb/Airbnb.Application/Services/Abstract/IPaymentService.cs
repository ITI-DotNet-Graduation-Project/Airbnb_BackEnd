using Airbnb.Application.DTOs.Payment;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Repos.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airbnb.Infrastructure.Repos;
using Airbnb.Application.DTOs;

namespace Airbnb.Application.Services.Abstract
{
  
        public interface IPaymentService
        {
            Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
            Task<PaymentDTO> GetPaymentByIdAsync(int id);
            Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto);
            Task<PaymentDTO> UpdatePaymentAsync(PaymentDTO paymentDto);
            Task DeletePaymentAsync(int id);
        }

       
}
