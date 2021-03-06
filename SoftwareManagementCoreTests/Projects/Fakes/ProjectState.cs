﻿using ProjectsShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreTests.Fakes
{
    public class ProjectState : IProjectState
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
    }
}
