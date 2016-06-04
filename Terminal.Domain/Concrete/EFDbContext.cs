using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Domain.Entities;

namespace Terminal.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("TerminalConnection") { }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
