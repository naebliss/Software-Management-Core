﻿using CommandsShared;
using Microsoft.EntityFrameworkCore;
using ProductsShared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;
using EmploymentsShared;

namespace SoftwareManagementEFCoreRepository
{
    public class MainContext : DbContext
    {
        public MainContext()
        {

        }
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
        }
        public DbSet<ProductState> ProductStates { get; set; }
        public DbSet<ProjectState> ProjectStates { get; set; }
        public DbSet<ProjectRoleState> ProjectRoleStates { get; set; }
        public DbSet<CommandState> CommandStates { get; set; }
        public DbSet<ContactState> ContactStates { get; set; }
        public DbSet<CompanyState> CompanyStates { get; set; }
        public DbSet<CompanyRoleState> CompanyRoleStates { get; set; }
        public DbSet<EmploymentState> EmploymentStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectState>()
                .HasMany(h => (ICollection<ProjectRoleState>)h.ProjectRoleStates)
                .WithOne()
                .HasForeignKey(p => p.ProjectGuid);

            modelBuilder.Entity<CompanyState>()
                .HasMany(h => (ICollection<CompanyRoleState>)h.CompanyRoleStates)
                .WithOne()
                .HasForeignKey(p => p.CompanyGuid);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost;Database=SoftwareManagement;Trusted_Connection=True;");
                //               optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
        }
    }
    public abstract class NamedEntityState : INamedEntityState
    {
        [Key]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Name { get; set; }
    }

    public class ProductState : NamedEntityState, IProductState
    {
        public string Description { get; set; }
        public string BusinessCase { get; set; }
    }

    public class ProjectRoleState : NamedEntityState, IProjectRoleState
    {
        public Guid ProjectGuid { get; set; }
    }

    public class ContactState : NamedEntityState, IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
    }

    public class CompanyState : NamedEntityState, ICompanyState
    {
        public CompanyState()
        {
            CompanyRoleStates = new List<ICompanyRoleState>() as ICollection<ICompanyRoleState>;
        }
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
    }

    public class CompanyRoleState : NamedEntityState, ICompanyRoleState
    {
        public Guid CompanyGuid { get; set; }
    }

    public class ProjectState : NamedEntityState, IProjectState
    {
        public ProjectState()
        {
            ProjectRoleStates = new List<IProjectRoleState>() as ICollection<IProjectRoleState>;
        }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
    }

    public class EmploymentState : IEmploymentState
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CommandState : ICommandState
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public string CommandTypeId { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
    }
    public interface IMainRepository : IProductStateRepository, IContactStateRepository,
        IProjectStateRepository, ICompanyStateRepository, ICommandStateRepository, IEmploymentStateRepository
    { };
    public class MainRepository : IMainRepository
    {
        private MainContext _context;
        public MainRepository(MainContext context)
        {
            _context = context;
            //if (_context.Database.GetPendingMigrations().Any())
            //{
            //    _context.Database.Migrate();
            //}
        }

        public void AddCommandState(ICommandState state)
        {
            throw new NotImplementedException();
        }

        public ICommandState CreateCommandState()
        {
            var state = new CommandState()
            {
                Guid = Guid.NewGuid()
            };
            _context.CommandStates.Add(state);
            return state;
        }

        public void DeleteProjectState(Guid guid)
        {
            var state = GetProjectState(guid) as ProjectState;
            _context.ProjectStates.Remove(state);
        }

        public void DeleteProductState(Guid guid)
        {
            var state = GetProductState(guid) as ProductState;
            _context.ProductStates.Remove(state);
        }

        public IProductState CreateProductState(Guid guid, string name)
        {
            var state = new ProductState()
            {
                Guid = guid,
                Name = name
            };
            _context.ProductStates.Add(state);
            return state;
        }

        public IProjectState CreateProjectState(Guid guid, string name)
        {
            var state = new ProjectState()
            {
                Guid = guid,
                Name = name
            };
            _context.ProjectStates.Add(state);
            return state;
        }

        public bool Exists(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllNew()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllProcessed()
        {
            throw new NotImplementedException();
        }

        public IProductState GetProductState(Guid guid)
        {
            return _context.ProductStates.Find(guid);
        }

        public IProjectState GetProjectState(Guid guid)
        {
            return _context.ProjectStates.Include(s => s.ProjectRoleStates).SingleOrDefault(s => s.Guid == guid);
        }

        public IProjectState GetProjectStateReadOnly(Guid guid)
        {
            return _context.ProjectStates.Include(s => s.ProjectRoleStates).AsNoTracking().SingleOrDefault(s => s.Guid == guid);
        }


        public IEnumerable<IProductState> GetProductStates()
        {
            // todo: make a separate readonly repo for the query part of CQRS
            return _context.ProductStates.AsNoTracking().ToList();
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            // todo: make a separate readonly repo for the query part of CQRS
            return _context.ProjectStates.Include(s => s.ProjectRoleStates).AsNoTracking().ToList();
        }

        public IProjectRoleState CreateProjectRoleState(Guid projectGuid, Guid guid, string name)
        {
            var newState = new ProjectRoleState { ProjectGuid = projectGuid, Guid = guid, Name = name };
            _context.ProjectRoleStates.Add(newState);
            return newState;

        }

        public void AddRoleToProjectState(Guid projectGuid, Guid projectRoleGuid, string projectRoleName)
        {
            var projectState = GetProjectState(projectGuid);

            if (projectState.ProjectRoleStates.All(w => w.Guid != projectRoleGuid))
            {
                CreateProjectRoleState(projectGuid, projectRoleGuid, projectRoleName);
            }
        }

        public void RemoveRoleFromProjectState(Guid projectGuid, Guid projectRoleGuid)
        {
            var projectState = GetProjectState(projectGuid);
            var projectRoleState = projectState.ProjectRoleStates.SingleOrDefault(s => s.Guid == projectRoleGuid);
            if (projectRoleState != null)
            {
                projectState.ProjectRoleStates.Remove(projectRoleState);
                _context.ProjectRoleStates.Remove((ProjectRoleState)projectRoleState);
            }
        }

        public ICompanyRoleState CreateCompanyRoleState(Guid companyGuid, Guid guid, string name)
        {
            var newState = new CompanyRoleState { CompanyGuid = companyGuid, Guid = guid, Name = name };
            _context.CompanyRoleStates.Add(newState);
            return newState;

        }

        public void AddRoleToCompanyState(Guid companyGuid, Guid companyRoleGuid, string companyRoleName)
        {
            var companyState = GetCompanyState(companyGuid);

            if (companyState.CompanyRoleStates.All(w => w.Guid != companyRoleGuid))
            {
                CreateCompanyRoleState(companyGuid, companyRoleGuid, companyRoleName);
            }
        }

        public void RemoveRoleFromCompanyState(Guid companyGuid, Guid companyRoleGuid)
        {
            var companyState = GetCompanyState(companyGuid);
            var companyRoleState = companyState.CompanyRoleStates.SingleOrDefault(s => s.Guid == companyRoleGuid);
            if (companyRoleState != null)
            {
                companyState.CompanyRoleStates.Remove(companyRoleState);
                _context.CompanyRoleStates.Remove((CompanyRoleState)companyRoleState);
            }
        }

        public IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp)
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            _context.SaveChanges();
        }

        public Task PersistChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void SetProcessed(ICommandState state)
        {
            //            state.UserName = 
            //throw new NotImplementedException();
        }

        public IContactState CreateContactState(Guid guid, string name)
        {
            var state = new ContactState { Guid = guid, Name = name };
            _context.ContactStates.Add(state);
            return state;
        }

        public IContactState GetContactState(Guid guid)
        {
            var state = _context.ContactStates.Find(guid);
            return state;
        }

        public IEnumerable<IContactState> GetContactStates()
        {
            return _context.ContactStates.AsNoTracking().ToList();
        }

        public void DeleteContactState(Guid guid)
        {
            var state = _context.ContactStates.Find(guid);
            _context.ContactStates.Remove(state);
        }

        public ICompanyState CreateCompanyState(Guid guid, string name)
        {
            var state = new CompanyState { Guid = guid, Name = name };
            _context.CompanyStates.Add(state);
            return state;
        }

        public ICompanyState GetCompanyState(Guid guid)
        {
            var state = _context.CompanyStates.Include(i => i.CompanyRoleStates).SingleOrDefault(s => s.Guid == guid);
            return state;
        }

        public IEnumerable<ICompanyState> GetCompanyStates()
        {
            return _context.CompanyStates.Include(i => i.CompanyRoleStates).AsNoTracking().ToList();
        }

        public void DeleteCompanyState(Guid guid)
        {
            var state = _context.CompanyStates.Find(guid);
            _context.CompanyStates.Remove(state);
        }

        public IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid)
        {
            var state = new EmploymentState()
            {
                Guid = guid,
                ContactGuid = contactGuid,
                CompanyRoleGuid = companyRoleGuid
            };
            _context.EmploymentStates.Add(state);
            return state;
        }

        public IEmploymentState GetEmploymentState(Guid guid)
        {
            return _context.EmploymentStates.Find(guid);
        }

        public void DeleteEmploymentState(Guid entityGuid)
        {
            var state = GetEmploymentState(entityGuid) as EmploymentState;
            _context.EmploymentStates.Remove(state);
        }

        public ICollection<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            var states = _context.EmploymentStates.AsNoTracking().Where(w => w.CompanyRoleGuid == companyRoleGuid).ToList();
            return states as ICollection<IEmploymentState>;
        }

        public ICollection<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid)
        {
            var states = _context.EmploymentStates.AsNoTracking().Where(w => w.ContactGuid == contactGuid).ToList();
            return states as ICollection<IEmploymentState>;
        }

        public ICollection<IEmploymentState> GetEmploymentStates()
        {
            var states = _context.EmploymentStates.AsNoTracking().ToList();
            return states as ICollection<IEmploymentState>;
        }

        public IEnumerable<ICommandState> GetCommandStates(Guid guid)
        {
            // todo: consider which date I want to use. Ideally the created on reflects the time the user created the command correctly. Ideally ... 
            var states = _context.CommandStates.Where(w => w.EntityGuid == guid).OrderByDescending(o=>o.CreatedOn).AsNoTracking().ToList();
            return states;
        }
    }
}
