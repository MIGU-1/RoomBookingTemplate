using Microsoft.EntityFrameworkCore;
using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomBooking.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
          => await _dbContext.Customers
              .OrderBy(customers => customers.LastName)
              .ToListAsync();

        public async Task<IEnumerable<Customer>> GetAllWithBookingsAndRoomsAsync()
          => await _dbContext.Customers
              .Include("Bookings.Room")
              .OrderBy(customers => customers.LastName)
              .ToListAsync();

        public async Task<Customer> GetByIdAsync(int id)
          => await _dbContext.Customers.FindAsync(id);

        public void Update(Customer customer)
        {
            Customer customerInDb = _dbContext
                .Customers
                .Where(c => c.Id == customer.Id)
                .SingleOrDefault();

            customerInDb.FirstName = customer.FirstName;
            customerInDb.LastName = customer.LastName;
            customerInDb.Iban = customer.Iban;

            _dbContext.SaveChanges();
        }
    }
}
