using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Repositories.Concrete
{
    public class PaymentRepository : IPaymentRepository
    {
        private ProjectContext _context;

        public PaymentRepository(ProjectContext context)
        {
            _context = context;
        }
        public IQueryable<Payment> Payments => _context.Payments;

        public bool AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            return _context.SaveChanges() > 0;
        }
        public IEnumerable<Payment> GetAll()
        {
            return _context.Payments.Where(x => x.IsActived == true).ToList();
        }
        public Payment GetById(int id)
        {
            return _context.Payments.Find(id);
        }
        public bool DeletePayment(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }
        public bool UpdatePayment(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool AddPayment(InsertPaymentVM paymentVM)
        {
            
            Payment payment = new Payment()
            {
                Amount = paymentVM.Amount,
                Description = paymentVM.Description,
                CreatedDate = DateTime.Now,
                RequestDate=DateTime.Now,
                IsActived = false,
                IsDeleted = false,
                IsApproved = false,
                PersonnelID = paymentVM.PersonelID
            };
            _context.Payments.Add(payment);
            return _context.SaveChanges() > 0;
        }

        public bool UpdatePayment(InsertPaymentVM paymentVM)
        {
            throw new NotImplementedException();
        }

    }
}
