## TETACOM .NET opensource NuGet packages
![master build status](https://github.com/tetacom-software/Teta.Net.Packages/actions/workflows/dotnet-core.yml/badge.svg?branch=master)
![dotnet](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

This set of  libraries will help you start new .NET Core web api project with common-used application-design patterns 

##### Package contents
* [EfCore UnitOfWork](#efcoreunitofwork)
* (WIP) [EfCore UnitOfWork: Repository usage](#efcoreunitofworkrepousage)

#### <a name="efcoreunitofwork"></a> "Unit of Work" and "Repository" patterns for EFCore

First things first, you should install this package fro, NuGet using following command:

```
  nuget install Teta.Packages.UoW.EfCore
```

Next, UnitOfWork and EntityFramework assumes that you are using some DbContext implementation for your models;
In this case you should inherit your model from DbContext class and additionaly from your ICustomDbContext, for example:

```csharp

    /// <summary>
    /// My custom db context wtih models set-up
    /// </summary>
    public class MyDbContext : DbContext, IMyCustomContext, ISomeOtherDbContext
    {
        /// <summary>
        /// Initizalize context
        /// </summary>
        /// <param name="options">Context init options</param>
        public RirDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          // Setup your model relations
        }
```

After you set-up your DbContext you can add UnitOfWork to you DI container instance in Startup.cs, inside ConfigureServices section.
In our sample we are using postgres NpgSql driver for dbContext configuration

```csharp
        // This gheneric factory registration REQUIRED for  IGenericRepository inhection
        // Or you should register your custom implementation for IGenericRepository for each DbSet type
        services.AddScoped(typeof(IGenericRepository<,>), typeof(FactoryBasedGenericRepository<,>));
        
        services.RegisterUnitOfWork<IMyCustomContext, MyDbContext>(dbContextConfig =>
            {
                // Getting postgres connection string for application settings 
                var postgresConfig = configRoot.GetSection(nameof(Postgres)).Get<Postgres>();
                
                // Setup db context for using NpgSql postgres driver
                dbContextConfig.UseNpgsql(postgresConfig.ConnectionString);
            });
 ```
 
 Note, every model that you will use in your DbSet with this pattern should implement IBusinessEntity interface, for example;
 IBusinessEntity is the generic interface where Tkey - is the entity primary key type.
 
```csharp
    /// <summary>
    /// My entity
    /// </summary>
    [Table("entity_tab_name", Schema = "myschema")]
    public class MyEntity : BaseEntity, IBusinessEntity<int>
    {
        /// <summary>
        /// Pk
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        
        
        [Column("data")]
        public string Data { get; set; }
    }
```


After that you should be able to inject IGenericRepository<MyEntity> to any constructor of type, that has registrated inside your DI container;
Also you can inject IUnitOfWorkBase or specific implementation of Unit of work
 
For example in out case:
  
 
```csharp
  
    public class SampleService : IMySampleService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IGenericRepository<MyEntity, int> _myEntityRepository;

        /// <summary>
        /// Initializes new class instance <see cref="SampleService"/>.
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="myEntiryRepository">My entities repository</param>
        public SampleService(
            IUnitOfWorkBase unitOfWork,
            IGenericRepository<MyEntity, int> myEntiryRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _myEntityRepository = myEntiryRepository ?? throw new ArgumentNullException(nameof(myEntiryRepository));
        }
        
        /// <inheritdoc/>
        public async Task<Casing> GetAsync(int id)
        {
            return await _myEntityRepository.GetByIdAsync(id);
        }
     }
```
#### <a name="efcoreunitofworkrepousage"></a> Code samples with repository and unit of work pattern
(WIP)
