﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmploymentsShared;

// For more information on enabling Web API for empty employments, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class EmploymentDto
    {
        private IEmploymentState _state;
        public EmploymentDto(IEmploymentState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public Guid ContactGuid { get { return _state.ContactGuid; } }
        public Guid CompanyRoleGuid { get { return _state.CompanyRoleGuid; } }
        public string StartDate { get { return _state.StartDate.HasValue ? _state.StartDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public string EndDate { get { return _state.EndDate.HasValue ? _state.EndDate.Value.ToString("yyyy-MM-dd") : ""; } }
    }

    [Route("api/[controller]")]
    public class EmploymentsController : Controller
    {
        private IEmploymentStateRepository _employmentStateRepository;

        public EmploymentsController(IEmploymentStateRepository employmentStateRepository)
        {
            _employmentStateRepository = employmentStateRepository;
        }
        // GET: api/employments
        [HttpGet]
        public IEnumerable<EmploymentDto> Get()
        {
            var states = _employmentStateRepository.GetEmploymentStates();
            var dtos = states.Select(s => new EmploymentDto(s)).ToList();
            return dtos;
        }

        // GET: api/employments/getbycompanyroleid/5
        [HttpGet("/getbycompanyroleid/{guid}")]
        public IEnumerable<EmploymentDto> GetByCompanyRoleId(Guid guid)
        {
            var states = _employmentStateRepository.GetEmploymentsByCompanyRoleGuid(guid);
            var dtos = states.Select(s => new EmploymentDto(s)).ToList();
            return dtos;
        }

        // GET: api/employments/getbycontactid/5
        [HttpGet("/getbycontactid/{guid}")]
        public IEnumerable<EmploymentDto> GetByContactId(Guid guid)
        {
            var states = _employmentStateRepository.GetEmploymentsByContactGuid(guid);
            var dtos = states.Select(s => new EmploymentDto(s)).ToList();
            return dtos;
        }

        // GET api/employments/5
        [HttpGet("{guid}")]
        public EmploymentDto Get(Guid guid)
        {
            var state = _employmentStateRepository.GetEmploymentState(guid);
            return new EmploymentDto(state);
        }
    }
}
