using LIN.Types.Cloud.OpenAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.OpenAssistant.Persistence.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    /// <summary>
    /// Services.
    /// </summary>
    public required DbSet<EmmaService> Services { get; set; }


    /// <summary>
    /// Profiles.
    /// </summary>
    public required DbSet<ProfileModel> Profiles { get; set; }


    /// <summary>
    /// On Creating.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Services.
        modelBuilder.Entity<EmmaService>(et =>
        {
            et.ToTable("services");
        });

        // Perfiles.
        modelBuilder.Entity<ProfileModel>(et =>
        {
            et.HasIndex(t => t.AccountId).IsUnique();
            et.ToTable("profiles");
        });

        base.OnModelCreating(modelBuilder);
    }

}