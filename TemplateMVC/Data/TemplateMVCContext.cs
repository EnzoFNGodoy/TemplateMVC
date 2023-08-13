using Microsoft.EntityFrameworkCore;
using TemplateMVC.Models;

namespace TemplateMVC.Data;

public sealed class TemplateMVCContext : DbContext
{
    public TemplateMVCContext(DbContextOptions<TemplateMVCContext> options) 
        : base(options)
    { }

    // Registro da Tabela
    public DbSet<Customer> Customers { get; set; }
}