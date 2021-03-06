﻿using CompaniesShared;
using ContactsShared;
using ProductsShared;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareManagementEventSourceRepository
{
    public class ContactState : IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class CompanyState : ICompanyState
    {
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class CompanyRoleState : ICompanyRoleState
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class ProjectState : IProjectState
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class ProjectRoleState : IProjectRoleState
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class ProductState : IProductState
    {
        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class EventSourcedMainRepository : IContactStateRepository, ICompanyStateRepository, IProjectStateRepository, IProductStateRepository
    {
        private Dictionary<Guid, IContactState> _contactDictionary;
        private Dictionary<Guid, IProjectState> _projectDictionary;
        private Dictionary<Guid, ICompanyState> _companyDictionary;
        private Dictionary<Guid, IProductState> _productDictionary;
        public EventSourcedMainRepository()
        {
            // works as a nice cache too
            _contactDictionary = new Dictionary<Guid, IContactState>();
            _projectDictionary = new Dictionary<Guid, IProjectState>();
            _companyDictionary = new Dictionary<Guid, ICompanyState>();
            _productDictionary = new Dictionary<Guid, IProductState>();
        }

        public void AddRoleToCompanyState(Guid guid, Guid roleGuid, string name)
        {
            var state = GetCompanyState(guid);
            state.CompanyRoleStates.Add(new CompanyRoleState { Guid = roleGuid, Name = name });
        }

        public void AddRoleToProjectState(Guid guid, Guid roleGuid, string name)
        {
            var state = GetProjectState(guid);
            state.ProjectRoleStates.Add(new ProjectRoleState { Guid = roleGuid, Name = name });
        }

        public ICompanyState CreateCompanyState(Guid guid, string name)
        {
            ICompanyState state;
            if (_companyDictionary.TryGetValue(guid, out state))
            {
                state.Name = name; // todo: do we need concurrency check?
            }
            else
            {
                state = new CompanyState { Guid = guid, Name = name };
                _companyDictionary.Add(guid, state);
            }
            return state;
        }

        public IContactState CreateContactState(Guid guid, string name)
        {
            // in eventsourcing, state may already exist
            IContactState state;
            if(_contactDictionary.TryGetValue(guid, out state))
            {
                state.Name = name; // todo: do we need concurrency check?
            }
            else
            {
                state = new ContactState { Guid = guid, Name = name };
                _contactDictionary.Add(guid, state);
            }
            return state;
        }

        public IProductState CreateProductState(Guid guid, string name)
        {
            IProductState state;
            if (_productDictionary.TryGetValue(guid, out state))
            {
                state.Name = name; // todo: do we need concurrency check?
            }
            else
            {
                state = new ProductState { Guid = guid, Name = name };
                _productDictionary.Add(guid, state);
            }
            return state;
        }

        public IProjectState CreateProjectState(Guid guid, string name)
        {
            IProjectState state;
            if (_projectDictionary.TryGetValue(guid, out state))
            {
                state.Name = name; // todo: do we need concurrency check?
            }
            else
            {
                state = new ProjectState{ Guid = guid, Name = name };
                _projectDictionary.Add(guid, state);
            }
            return state;
        }

        public void DeleteCompanyState(Guid guid)
        {
//            throw new NotImplementedException();
        }

        public void DeleteContactState(Guid guid)
        {
            // ignore in eventsourcing
        }

        public void DeleteProductState(Guid guid)
        {
            // ignore in eventsourcing
        }

        public void DeleteProjectState(Guid guid)
        {
            //throw new NotImplementedException();
        }

        public ICompanyState GetCompanyState(Guid guid)
        {
            ICompanyState state;
            if (_companyDictionary.TryGetValue(guid, out state))
            {
                return state;
            }
            // in eventsourcing, we'll just create a nameless state
            state = CreateCompanyState(guid, "");
            return state;
        }

        public IEnumerable<ICompanyState> GetCompanyStates()
        {
            throw new NotImplementedException();
        }

        public IContactState GetContactState(Guid guid)
        {
            IContactState state;
            if (_contactDictionary.TryGetValue(guid, out state))
            {
                return state;
            }
            // in eventsourcing, we'll just create a nameless state
            state = CreateContactState(guid, "");
            return state;
        }

        public IEnumerable<IContactState> GetContactStates()
        {
            // we may need this at some point, but probably not
            throw new NotImplementedException();
        }

        public IProductState GetProductState(Guid guid)
        {
            IProductState state;
            if (_productDictionary.TryGetValue(guid, out state))
            {
                return state;
            }
            // in eventsourcing, we'll just create a nameless state
            state = CreateProductState(guid, "");
            return state;
        }

        public IEnumerable<IProductState> GetProductStates()
        {
            throw new NotImplementedException();
        }

        public IProjectState GetProjectState(Guid guid)
        {
            IProjectState state;
            if (_projectDictionary.TryGetValue(guid, out state))
            {
                return state;
            }
            // in eventsourcing, we'll just create a nameless state
            state = CreateProjectState(guid, "");
            return state;
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            // we don't persist state - we're always rebuilding from persisted events instead
        }

        public Task PersistChangesAsync()
        {
            // we don't persist state - we're always rebuilding from persisted events instead
            return Task.CompletedTask;
        }

        public void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid)
        {
            var state = GetCompanyState(guid);
            var roleState = state.CompanyRoleStates.Single(s => s.Guid == guid);
            state.CompanyRoleStates.Remove(roleState);
        }

        public void RemoveRoleFromProjectState(Guid guid, Guid roleGuid)
        {
            var state = GetProjectState(guid);
            var roleState = state.ProjectRoleStates.Single(s => s.Guid == guid);
            state.ProjectRoleStates.Remove(roleState);
        }
    }
}
