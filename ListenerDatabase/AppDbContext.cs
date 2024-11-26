using ListenerDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDatabase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
   
    public AppDbContext() { 
    }
    public DbSet<User> Users { get; set; }
    public DbSet<KeyLog> KeyLogs { get; set; }
    public DbSet<Screenshot> Screenshots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Замініть connection string своїм
        optionsBuilder.UseSqlServer("Data Source=IHORNOTEBOOK;Initial Catalog=ListenerDatabase;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=False;Trust Server Certificate=False;Command Timeout=0");
    }
}