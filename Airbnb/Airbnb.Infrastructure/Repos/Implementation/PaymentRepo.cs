using Airbnb.DATA.models;
using Airbnb.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Infrastructure.Repos.Implementation
{
    
        public class PaymentRepo : GenericRepo<Payment>
        {
            private readonly AppDbContext _context;

            public PaymentRepo(AppDbContext context) : base(context)
            {
                _context = context;
            }

            // You can add Payment-specific methods here if needed
        }
}
