﻿using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProjectsShared
{
    public abstract class ProjectCommand : CommandBase
    {
        public ProjectCommand() : base() { }
        public ProjectCommand(ICommandRepository repo) : base(repo) { }
    }

    public class CreateProjectCommand : ProjectCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            ((IProjectService)base.CommandProcessor).CreateProject(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameProjectCommand : ProjectCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            product.Rename(this.Name);
            base.Execute();
        }
    }

    public class ChangeStartDateOfProjectCommand : ProjectCommand
    {
        public DateTime? OriginalStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.ChangeStartDate(this.StartDate);
            base.Execute();
        }
    }

    public class ChangeEndDateOfProjectCommand : ProjectCommand
    {
        public DateTime? OriginalEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.ChangeEndDate(this.EndDate);
            base.Execute();
        }
    }
}