// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitDbContext.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Unit measure db context
// </summary>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Teta.Packages.UnitMeasure.Contracts;

namespace Teta.Packages.UnitMeasure
{
    /// <summary>
    /// Unit measure db context
    /// </summary>
    public class UnitDbContext : DbContext, IUnitDbContext
    {
        /// <summary>
        /// Initializes a new instance of the class <see cref="UnitDbContext"/>.
        /// </summary>
        /// <param name="options">options</param>
        public UnitDbContext(DbContextOptions<UnitDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var options = this.GetService<IOptions<UnitMeasureOptions>>();
            if (options == null)
            {
                throw new NullReferenceException(nameof(options));
            }

            modelBuilder.HasDefaultSchema(options.Value.SchemaName);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CalcParameter>()
                .HasOne(c => c.UnitGroup)
                .WithMany()
                .HasForeignKey(e => e.UnitGroupId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<CalcUnit>()
                .HasOne(cu => cu.Convertation)
                .WithOne()
                .HasForeignKey<CalcUnitConvert>(cu => cu.UnitId);

            modelBuilder.Entity<UnitGroup>()
                .HasMany(g => g.Units)
                .WithOne()
                .HasForeignKey(u => u.UnitGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeasureSystem>()
                .HasMany(e => e.MeasureSystemParameters)
                .WithOne()
                .HasForeignKey(e => e.MeasureSystemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCalcParameterWithUnit>()
                .HasOne(p => p.CalcParameter)
                .WithMany()
                .HasForeignKey(p => p.ParameterId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<UserCalcParameterWithUnit>()
                .HasOne(p => p.Unit)
                .WithMany()
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }

        /// <inheritdoc/>
        public DbContext DbContext => this;

        /// <summary>
        /// Calc parameter set
        /// </summary>
        public DbSet<CalcParameter> CalcParameters { get; set; }

        /// <summary>
        /// Calc unit set
        /// </summary>
        public DbSet<CalcUnit> Units { get; set; }

        /// <summary>
        /// Unit group set
        /// </summary>
        public DbSet<UnitGroup> Groups { get; set; }

        /// <summary>
        /// Unit convertation set
        /// </summary>
        public DbSet<CalcUnitConvert> UnitsConvert { get; set; }

        /// <summary>
        /// Default measure system set
        /// </summary>
        public DbSet<MeasureSystem> MeasureSystems { get; set; }

        /// <summary>
        /// Measure system parameter
        /// </summary>
        public DbSet<MeasureSystemCalcParameter> MeasureSystemCalcParameters { get; set; }

        /// <summary>
        /// User settings
        /// </summary>
        public DbSet<UserCalcParameterWithUnit> UserCalcParameterWithUnit { get; set; }
    }
}